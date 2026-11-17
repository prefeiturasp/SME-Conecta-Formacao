using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafConferencias;
using SME.ConectaFormacao.Infra.Dados.Queries;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafConferencia(IConectaFormacaoConexao conexao, IContextoAplicacao contexto) :
        RepositorioBaseAuditavel<CodafConferencia>(contexto, conexao), IRepositorioCodafConferencia
    {
        public async Task<bool> TurmaJaTemCodafAsync(long propostaTurmaId, long? codafConferenciaId = null)
        {
            const string query = """
                SELECT 1
                FROM PUBLIC.CODAF_CONFERENCIA
                WHERE PROPOSTA_TURMA_ID = @propostaTurmaId
                  AND (@codafConferenciaId IS NULL OR ID <> @codafConferenciaId)
                  AND NOT EXCLUIDO
                LIMIT 1
                """;

            return await conexao.Obter().QueryFirstOrDefaultAsync<bool>(query, new { propostaTurmaId, codafConferenciaId });
        }

        public async Task<ResultadoPaginado<ListagemResultadoCodafConferenciaDto>> ObterListagemPorFiltroAsync(FiltroListagemResultadoCodafConferenciaDto filtro)
        {
            const string sqlBaseJoins = """
                FROM   PUBLIC.CODAF_CONFERENCIA AS CC
                INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CC.PROPOSTA_TURMA_ID = PT.ID
                INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID 
                INNER JOIN PUBLIC.AREA_PROMOTORA AS AP ON P.AREA_PROMOTORA_ID = AP.ID
                """;
            const string sqlBaseOrderBy = """
                ORDER  BY
                        CASE WHEN CC.DATA_FINALIZACAO IS NULL THEN 0
                             ELSE 1
                        END DESC,
                        CC.DATA_FINALIZACAO ASC,
                        CC.CRIADO_EM DESC
                """;

            var condicoesWhere = new StringBuilder("WHERE NOT CC.EXCLUIDO AND NOT PT.EXCLUIDO AND NOT P.EXCLUIDO ");
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
                condicoesWhere.Append(" AND CC.STATUS = @status ");
                parametros.Add("status", filtro.Status.Value);
            }

            if (filtro.DataCriacao is not null)
            {
                condicoesWhere.Append(" AND DATE(CC.CRIADO_EM) = DATE(@dataCriacao) ");
                parametros.Add("dataCriacao", filtro.DataCriacao.Value);
            }

            var conn = conexao.Obter();
            var sqlCount = new StringBuilder($"""
                SELECT COUNT(1)
                {sqlBaseJoins}
                {condicoesWhere}
                """);

            var totalRegistros = await conn.QueryFirstAsync<int>(sqlCount.ToString(), parametros);
            if (totalRegistros == 0)
                return new ResultadoPaginado<ListagemResultadoCodafConferenciaDto>
                {
                    Itens = [],
                    PaginaAtual = filtro.Pagina,
                    TamanhoPagina = filtro.TamanhoPagina,
                    TotalRegistros = 0
                };

            var registrosIgnorados = (filtro.Pagina - 1) * filtro.TamanhoPagina;
            parametros.Add("limite", filtro.TamanhoPagina);
            parametros.Add("registrosIgnorados", registrosIgnorados);

            var sqlConsulta = new StringBuilder($"""
                {CodafConferenciaQueries.sqlObterListagemCodaf}
                {sqlBaseJoins}
                {condicoesWhere}
                {sqlBaseOrderBy}
                LIMIT @limite OFFSET @registrosIgnorados
                """);

            var itens = await conn.QueryAsync<ListagemResultadoCodafConferenciaDto>(sqlConsulta.ToString(), parametros);
            return new ResultadoPaginado<ListagemResultadoCodafConferenciaDto>
            {
                Itens = itens,
                PaginaAtual = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina,
                TotalRegistros = totalRegistros
            };
        }

        public async Task<CodafConferencia?> ObterPorIdDetalhadoAsync(long id)
        {
            var conn = conexao.Obter();
            var sql = $"""
                {CodafConferenciaQueries.sqlObterCodafPorIdComPropostaEPropostaTurma}
                {CodafConferenciaQueries.sqlObterAnexosPorIdCodaf}
                {CodafConferenciaQueries.sqlObterInscricoesDaListaPorIdCodaf}
                """;

            var parametros = new { id };

            using var multi = await conn.QueryMultipleAsync(sql, parametros);
            var codafConferencia = multi.Read<CodafConferencia, Proposta, PropostaTurma, CodafConferencia>(
                (cc, p, pt) =>
                {
                    cc.Proposta = p;
                    cc.PropostaTurma = pt;
                    return cc;
                },
                splitOn: "ID,ID").SingleOrDefault();

            if (codafConferencia == null)
                return null;

            codafConferencia.CodafAnexos = [.. await multi.ReadAsync<CodafConferenciaAnexo>()];
            codafConferencia.CodafInscricoes = [.. await multi.ReadAsync<CodafConferenciaInscricao>()];

            return codafConferencia;
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

                const string sqlCodafConferencia = """
                    UPDATE PUBLIC.CODAF_CONFERENCIA
                    SET EXCLUIDO = @Excluido,
                        ALTERADO_EM = @AlteradoEm,
                        ALTERADO_POR = @AlteradoPor,
                        ALTERADO_LOGIN = @AlteradoLogin
                    WHERE ID = @Id;
                    """;

                const string sqlAnexos = """
                    UPDATE PUBLIC.CODAF_CONFERENCIA_ANEXO
                    SET EXCLUIDO = @Excluido,
                        ALTERADO_EM = @AlteradoEm,
                        ALTERADO_POR = @AlteradoPor,
                        ALTERADO_LOGIN = @AlteradoLogin
                    WHERE CODAF_CONFERENCIA_ID = @Id;
                    """;

                const string sqlInscritos = """
                    UPDATE PUBLIC.CODAF_CONFERENCIA_INSCRICAO
                    SET EXCLUIDO = @Excluido,
                        ALTERADO_EM = @AlteradoEm,
                        ALTERADO_POR = @AlteradoPor,
                        ALTERADO_LOGIN = @AlteradoLogin
                    WHERE CODAF_CONFERENCIA_ID = @Id;
                    """;

                await conn.ExecuteAsync(sqlCodafConferencia, parametrosAtualizacao, transaction);
                await conn.ExecuteAsync(sqlAnexos, parametrosAtualizacao, transaction);
                await conn.ExecuteAsync(sqlInscritos, parametrosAtualizacao, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
