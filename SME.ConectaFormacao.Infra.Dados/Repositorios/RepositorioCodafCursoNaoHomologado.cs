using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Queries;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafCursoNaoHomologado(IConectaFormacaoConexao conexao, IContextoAplicacao contexto) :
        RepositorioBaseAuditavel<CodafCursoNaoHomologado>(contexto, conexao), IRepositorioCodafCursoNaoHomologado
    {
        public async Task<ResultadoPaginado<ListagemResultadoCodafCursoNaoHomologadoDto>> ObterListagemResultadoCodafCursoNaoHomologadoPorFiltroAsync(FiltroListagemResultadoCodafCursoNaoHomologadoDto filtro)
        {
            const string sqlBaseJoins = """
                FROM   PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO AS CCNH
                INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CCNH.PROPOSTA_TURMA_ID = PT.ID
                INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID 
                INNER JOIN PUBLIC.AREA_PROMOTORA AS AP ON P.AREA_PROMOTORA_ID = AP.ID
                """;
            const string sqlBaseOrderBy = """
                ORDER  BY
                        CASE WHEN CCNH.DATA_FINALIZACAO IS NULL THEN 0
                             ELSE 1
                        END DESC,
                        CCNH.DATA_FINALIZACAO ASC,
                        CCNH.CRIADO_EM DESC
                """;

            var condicoesWhere = new StringBuilder("WHERE NOT CCNH.EXCLUIDO AND NOT PT.EXCLUIDO AND NOT P.EXCLUIDO ");
            var parametros = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(filtro.NomeFormacao))
            {
                condicoesWhere.Append(" AND f_unaccent(P.NOME_FORMACAO) ILIKE f_unaccent(@nomeFormacao) ");
                parametros.Add("nomeFormacao", $"%{filtro.NomeFormacao}%");
            }

            if (!string.IsNullOrWhiteSpace(filtro.CodigoFormacao))
            {
                condicoesWhere.Append(" AND CAST(P.ID AS TEXT) ILIKE @codigoFormacao ");
                parametros.Add("codigoFormacao", $"{filtro.CodigoFormacao.Trim()}%");
            }

            if (!string.IsNullOrWhiteSpace(filtro.NumeroHomologacao))
            {
                condicoesWhere.Append(" AND CAST(P.NUMERO_HOMOLOGACAO AS TEXT) ILIKE @numeroHomologacao ");
                parametros.Add("numeroHomologacao", $"{filtro.NumeroHomologacao.Trim()}%");
            }

            if (filtro.PropostaTurmaId is not null)
            {
                condicoesWhere.Append(" AND PT.ID = @propostaTurmaId ");
                parametros.Add("propostaTurmaId", filtro.PropostaTurmaId.Value);
            }

            if (filtro.AreaPromotoraId is not null)
            {
                condicoesWhere.Append(" AND AP.ID = @areaPromotoraId ");
                parametros.Add("areaPromotoraId", filtro.AreaPromotoraId.Value);
            }

            if (filtro.Status is not null)
            {
                condicoesWhere.Append(" AND CCNH.STATUS = @status ");
                parametros.Add("status", filtro.Status.Value);
            }

            if (filtro.DataFinalizacao is not null)
            {
                condicoesWhere.Append(" AND DATE(CCNH.DATA_FINALIZACAO) = DATE(@dataFinalizacao) ");
                parametros.Add("dataFinalizacao", filtro.DataFinalizacao.Value);
            }

            var conn = conexao.Obter();
            var sqlCount = new StringBuilder($"""
                SELECT COUNT(1)
                {sqlBaseJoins}
                {condicoesWhere}
                """);

            var totalRegistros = await conn.QueryFirstAsync<int>(sqlCount.ToString(), parametros);
            if (totalRegistros == 0)
                return new ResultadoPaginado<ListagemResultadoCodafCursoNaoHomologadoDto>
                {
                    Itens = [],
                    PaginaAtual = filtro.Pagina,
                    TamanhoPagina = filtro.TamanhoPagina,
                    TotalRegistros = 0
                };

            var registrosIgnorados = (filtro.Pagina - 1) * filtro.TamanhoPagina;
            parametros.Add("limite", filtro.TamanhoPagina);
            parametros.Add("registrosIgnorados", registrosIgnorados);
            parametros.Add("statusPendente", StatusProcessamentoDeclaracaoCodaf.Pendente);
            parametros.Add("statusEmProcessamento", StatusProcessamentoDeclaracaoCodaf.EmProcessamento);
            parametros.Add("statusProcessadoComSucesso", StatusProcessamentoDeclaracaoCodaf.ProcessadoComSucesso);
            parametros.Add("statusProcessadoComErro", StatusProcessamentoDeclaracaoCodaf.ProcessadoComErro);

            var sqlConsulta = new StringBuilder($"""
                {CodafNaoHomologadoQueries.sqlObterListagemCodaf}
                {sqlBaseJoins}
                {condicoesWhere}
                {sqlBaseOrderBy}
                LIMIT @limite OFFSET @registrosIgnorados
                """);

            var itens = await conn.QueryAsync<ListagemResultadoCodafCursoNaoHomologadoDto>(sqlConsulta.ToString(), parametros);
            return new ResultadoPaginado<ListagemResultadoCodafCursoNaoHomologadoDto>
            {
                Itens = itens,
                PaginaAtual = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina,
                TotalRegistros = totalRegistros
            };
        }

        public async Task<CodafCursoNaoHomologado?> ObterPorIdDetalhadoAsync(long id)
        {
            var conn = conexao.Obter();
            var sql = $"""
                -- 1. Dados do Cabeçalho (CODAF + Proposta + Turma)
                {CodafNaoHomologadoQueries.sqlObterCodafPorIdComPropostaEPropostaTurma}

                -- 2. Anexos
                {CodafNaoHomologadoQueries.sqlObterAnexosPorIdCodaf}

                -- 3. Inscritos (A lista grande)
                {CodafNaoHomologadoQueries.sqlObterInscricoesDaListaPorIdCodaf}

                -- 4. Declarações
                {CodafNaoHomologadoQueries.sqlObterDeclaracoesPorIdCodaf}
                """;

            var parametros = new { id };

            using var multi = await conn.QueryMultipleAsync(sql, parametros);
            var codafCursoNaoHomologado = multi.Read<CodafCursoNaoHomologado, Proposta, PropostaTurma, CodafCursoNaoHomologado>(
            (clp, p, pt) =>
            {
                clp.Proposta = p;
                clp.PropostaTurma = pt;
                return clp;
            },
            splitOn: "ID,ID").SingleOrDefault();

            if (codafCursoNaoHomologado == null)
                return null;

            codafCursoNaoHomologado.CodafAnexos = [.. await multi.ReadAsync<CodafCursoNaoHomologadoAnexo>()];
            codafCursoNaoHomologado.CodafInscricoes = [.. await multi.ReadAsync<CodafCursoNaoHomologadoInscricao>()];
            codafCursoNaoHomologado.CodafDeclaracoes = [.. await multi.ReadAsync<CodafDeclaracao>()];

            return codafCursoNaoHomologado;
        }

        public async Task ExcluirAsync(long id)
        {
            var conn = conexao.Obter();
            using var transaction = conn.BeginTransaction();

            try
            {
                var parametrosAtualizacao = new
                {
                    Id = id,
                    Excluido = true,
                    AlteradoEm = DateTimeExtension.HorarioBrasilia(),
                    AlteradoPor = contexto.NomeUsuario,
                    AlteradoLogin = contexto.UsuarioLogado
                };

                const string sqlCodafCursoNaoHomologado = """
                    UPDATE PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO
                    SET    EXCLUIDO = @Excluido,
                           ALTERADO_EM = @AlteradoEm,
                           ALTERADO_POR = @AlteradoPor,
                           ALTERADO_LOGIN = @AlteradoLogin
                    WHERE  ID = @Id
                    """;
                await conn.ExecuteAsync(sqlCodafCursoNaoHomologado, parametrosAtualizacao, transaction);

                const string sqlInscricoes = """
                    UPDATE PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO
                    SET    EXCLUIDO = @Excluido,
                           ALTERADO_EM = @AlteradoEm,
                           ALTERADO_POR = @AlteradoPor,
                           ALTERADO_LOGIN = @AlteradoLogin
                    WHERE  CODAF_CURSO_NAO_HOM_ID = @Id and NOT EXCLUIDO
                    """;

                await conn.ExecuteAsync(sqlInscricoes, parametrosAtualizacao, transaction);

                const string sqlAnexos = """
                    UPDATE PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO_ANEXO
                    SET    EXCLUIDO = @Excluido,
                           ALTERADO_EM = @AlteradoEm,
                           ALTERADO_POR = @AlteradoPor,
                           ALTERADO_LOGIN = @AlteradoLogin
                    WHERE  CODAF_CURSO_NAO_HOM_ID = @Id and NOT EXCLUIDO
                    """;

                await conn.ExecuteAsync(sqlAnexos, parametrosAtualizacao, transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public override async Task<CodafCursoNaoHomologado?> ObterNaoExcluidosPorIdAsync(long id)
        {
            var conn = conexao.Obter();
            var sql = $"""
                -- 1. CODAF
                SELECT CCNH.ID as Id,
                       CCNH.PROPOSTA_ID AS propostaId,
                       CCNH.PROPOSTA_TURMA_ID AS propostaTurmaId,
                       CCNH.OBSERVACAO,
                       CCNH.STATUS,
                       CCNH.ALTERADO_EM AS alteradoEm,
                       CCNH.ALTERADO_POR AS alteradoPor,
                       CCNH.ALTERADO_LOGIN AS alteradoLogin,
                       CCNH.CRIADO_EM AS criadoEm,
                       CCNH.CRIADO_POR AS criadoPor,
                       CCNH.CRIADO_LOGIN AS criadoLogin
                FROM   PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO AS CCNH
                WHERE  NOT CCNH.EXCLUIDO AND CCNH.ID = @id;

                -- 5. Declaracoes
                {CodafNaoHomologadoQueries.sqlObterDeclaracoesPorIdCodaf}
                """;

            var parametros = new { id };

            using var multi = await conn.QueryMultipleAsync(sql, parametros);
            var codafCursoNaoHomologado = (await multi.ReadAsync<CodafCursoNaoHomologado>()).SingleOrDefault();

            if (codafCursoNaoHomologado is null)
                return null;

            codafCursoNaoHomologado.CodafDeclaracoes = [.. await multi.ReadAsync<CodafDeclaracao>()];
            return codafCursoNaoHomologado;
        }
    }
}
