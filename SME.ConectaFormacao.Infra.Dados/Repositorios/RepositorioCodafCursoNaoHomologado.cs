using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;
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

        public async Task<int> ObterStatusDeclaracaoTurmaAsync(long id)
        {
            var conn = conexao.Obter();
            const string sql = """
        SELECT
            CASE
                WHEN CCNH.STATUS = 1 THEN 1
                WHEN NOT EXISTS (SELECT 1
                                 FROM   CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO AS CCNHI
                                 WHERE  NOT CCNHI.EXCLUIDO
                                   AND  CCNHI.CODAF_CURSO_NAO_HOM_ID = CCNH.ID
                                   AND  CCNHI.PARTICIPOU) THEN 0
                WHEN EXISTS (SELECT 1
                             FROM   CODAF_DECLARACOES CD
                             WHERE  NOT CD.EXCLUIDO
                               AND  CD.CODAF_CURSO_NAO_HOMOLOGADO_ID = CCNH.ID
                               AND  CD.STATUS_PROCESSAMENTO IN (@statusPendente, @statusEmProcessamento)) THEN 3
                WHEN EXISTS (SELECT 1
                             FROM   CODAF_DECLARACOES CD
                             WHERE  NOT CD.EXCLUIDO
                               AND  CD.CODAF_CURSO_NAO_HOMOLOGADO_ID = CCNH.ID
                               AND  CD.STATUS_PROCESSAMENTO IN (@statusProcessadoComSucesso, @statusProcessadoComErro)) THEN 4
                ELSE 2
            END AS status
        FROM PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO AS CCNH
        WHERE NOT CCNH.EXCLUIDO AND CCNH.ID = @id
        """;

            var parametros = new DynamicParameters();
            parametros.Add("id", id);
            parametros.Add("statusPendente", StatusProcessamentoDeclaracaoCodaf.Pendente);
            parametros.Add("statusEmProcessamento", StatusProcessamentoDeclaracaoCodaf.EmProcessamento);
            parametros.Add("statusProcessadoComSucesso", StatusProcessamentoDeclaracaoCodaf.ProcessadoComSucesso);
            parametros.Add("statusProcessadoComErro", StatusProcessamentoDeclaracaoCodaf.ProcessadoComErro);

            return await conn.QueryFirstAsync<int>(sql, parametros);
        }

        public async Task<DadosPrincipaisRelatorioCodafCursoNaoHomologadoDto?> ObterDadosRelatorioAsync(long codafId)
        {
            const string sql = """
        -- Dados Principais da Turma
        SELECT DISTINCT
               CCNH.ID AS codafId,
               PT.ID AS turmaId,
               PT.NOME AS nomeTurma,
               P.QUANTIDADE_VAGAS_TURMA AS quantidadeVagasTurma,
               AP.NOME AS nomeAreaPromotora,
               P.TIPO_FORMACAO AS tipoFormacao,
               P.NOME_FORMACAO AS nomeFormacao,
               P.QUANTIDADE_TURMAS AS quantidadeTurmas,
               COALESCE(PGP.DATA_INICIO, P.DATA_REALIZACAO_INICIO) AS periodoRealizacaoInicio,
               COALESCE(PGP.DATA_FIM, P.DATA_REALIZACAO_FIM) AS periodoRealizacaoFim,
               P.CURSO_COM_CERTIFICADO AS cursoComCertificado,
               P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
               P.CODIGO_EVENTO_SIGPEC AS codigoEventoSigpec,
               CAST(
                    EXTRACT(HOUR FROM
                        CASE
                            WHEN P.CARGA_HORARIA_TOTAL_OUTRA IS NOT NULL AND P.CARGA_HORARIA_TOTAL_OUTRA <> ''
                            THEN P.CARGA_HORARIA_TOTAL_OUTRA::interval
                            ELSE COALESCE(NULLIF(P.CARGA_HORARIA_PRESENCIAL, ''), '00:00')::interval +
                                 COALESCE(NULLIF(P.CARGA_HORARIA_DISTANCIA, ''), '00:00')::interval
                        END
                    ) AS INTEGER
               ) AS cargaHorariaTotal,
               P.CARGA_HORARIA_DISTANCIA AS cargaHorariaDistancia,
               P.CARGA_HORARIA_SINCRONA AS cargaHorariaSincrona,
               P.CARGA_HORARIA_PRESENCIAL AS cargaHorariaPresencial,
               P.FORMATO AS tipoFormato,
               CASE WHEN D.DRE_ID IS NULL THEN '' ELSE D.NOME END AS nomeDre,
               CCNH.CRIADO_EM AS dataCodaf,
               CCNH.OBSERVACAO
        FROM   PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO AS CCNH
               INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON PT.ID = CCNH.PROPOSTA_TURMA_ID
               INNER JOIN PUBLIC.PROPOSTA AS P ON P.ID = PT.PROPOSTA_ID
               INNER JOIN PUBLIC.AREA_PROMOTORA AS AP ON AP.ID = P.AREA_PROMOTORA_ID
               LEFT JOIN PUBLIC.PROPOSTA_DRE AS PD ON PD.PROPOSTA_ID = P.ID
               LEFT JOIN PUBLIC.DRE AS D ON D.ID = PD.DRE_ID
               LEFT JOIN PUBLIC.PROPOSTA_GRUPO_PERIODO_TURMA PGPT ON PGPT.PROPOSTA_TURMA_ID = PT.ID AND NOT PGPT.EXCLUIDO
               LEFT JOIN PUBLIC.PROPOSTA_GRUPO_PERIODO PGP ON PGP.ID = PGPT.GRUPO_PERIODO_ID AND NOT PGP.EXCLUIDO
        WHERE  CCNH.ID = @codafId AND NOT CCNH.EXCLUIDO;

        -- Data das Aulas (mesma lógica do homologado — ligado direto por PROPOSTA_TURMA_ID)
        SELECT PED.DATA_INICIO AS dataInicio, PED.DATA_FIM AS dataFim
        FROM   PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO AS CCNH
               INNER JOIN PUBLIC.PROPOSTA_ENCONTRO_TURMA AS PET ON PET.TURMA_ID = CCNH.PROPOSTA_TURMA_ID
               INNER JOIN PUBLIC.PROPOSTA_ENCONTRO AS PE ON PE.ID = PET.PROPOSTA_ENCONTRO_ID
               INNER JOIN PUBLIC.PROPOSTA_ENCONTRO_DATA AS PED ON PED.PROPOSTA_ENCONTRO_ID = PE.ID
        WHERE  CCNH.ID = @codafId
          AND  PE.TIPO IN (@presencial, @sincrono)
          AND NOT PE.EXCLUIDO
          AND NOT PET.EXCLUIDO
          AND NOT PED.EXCLUIDO;

        -- Regentes (idêntico ao homologado)
        SELECT COALESCE(U.NOME, PR.NOME_REGENTE) AS nome,
               COALESCE(PR.REGISTRO_FUNCIONAL, PR.CPF) AS registroFuncional,
               CC.CODIGO_CERTIFICADO AS codigoCertificado
        FROM   PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO AS CCNH
               INNER JOIN PUBLIC.PROPOSTA_REGENTE_TURMA AS PRT ON PRT.TURMA_ID = CCNH.PROPOSTA_TURMA_ID
               INNER JOIN PUBLIC.PROPOSTA_REGENTE AS PR ON PR.ID = PRT.PROPOSTA_REGENTE_ID
               LEFT JOIN PUBLIC.USUARIO AS U ON U.LOGIN = PR.REGISTRO_FUNCIONAL AND NOT U.EXCLUIDO
               LEFT JOIN PUBLIC.CODAF_CERTIFICADOS AS CC ON CC.PROPOSTA_REGENTE_TURMA_ID = PRT.ID AND NOT CC.EXCLUIDO
        WHERE  CCNH.ID = @codafId
          AND  NOT CCNH.EXCLUIDO
          AND  NOT PRT.EXCLUIDO
          AND  NOT PR.EXCLUIDO;

        -- Participantes (schema PRÓPRIO do não homologado)
        SELECT U.LOGIN AS documento,
               (U.LOGIN <> U.CPF) AS temRf,
               U.NOME,
               CCNHI.PARTICIPOU AS participou,
               CD.CODIGO_DECLARACAO AS codigoCertificadoDeclaracao
        FROM   PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO AS CCNHI
               INNER JOIN PUBLIC.INSCRICAO AS I ON I.ID = CCNHI.INSCRICAO_ID
               INNER JOIN PUBLIC.USUARIO AS U ON U.ID = I.USUARIO_ID
               LEFT JOIN PUBLIC.CODAF_DECLARACOES AS CD
                      ON CD.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO_ID = CCNHI.ID
                     AND CD.TIPO_PARTICIPACAO = @tipoParticipacaoCursista
                     AND NOT CD.EXCLUIDO
        WHERE  CCNHI.CODAF_CURSO_NAO_HOM_ID = @codafId
          AND  NOT U.EXCLUIDO
          AND  NOT CCNHI.EXCLUIDO;
        """;

            var parametros = new
            {
                codafId,
                presencial = (int)TipoEncontro.Presencial,
                sincrono = (int)TipoEncontro.Sincrono,
                tipoParticipacaoCursista = (int)TipoParticipacaoCodaf.Cursista
            };

            var conn = conexao.Obter();
            using var multi = await conn.QueryMultipleAsync(sql, parametros);
            var dadosRelatorio = await multi.ReadFirstOrDefaultAsync<DadosPrincipaisRelatorioCodafCursoNaoHomologadoDto>();

            if (dadosRelatorio == null) return null;

            dadosRelatorio.DataAulas = await multi.ReadAsync<DataAulaTurmaRelatorioCodafDto>();
            dadosRelatorio.RegentesTurma = await multi.ReadAsync<DadosRegenteTurmaRelatorioCodafDto>();
            dadosRelatorio.Participantes = await multi.ReadAsync<DadosParticipanteRelatorioCodafCursoNaoHomologadoDto>();
            return dadosRelatorio;
        }
    }
}
