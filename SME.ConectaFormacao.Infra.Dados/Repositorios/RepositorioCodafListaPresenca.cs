using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Queries;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafListaPresenca(IConectaFormacaoConexao conexao, IContextoAplicacao contexto) :
        RepositorioBaseAuditavel<CodafListaPresenca>(contexto, conexao), IRepositorioCodafListaPresenca
    {
        public async Task<bool> TurmaJaTemListaDePresencaAsync(long propostaTurmaId, long listaPresencaId = 0)
        {
            const string query = """
                SELECT 1
                FROM CODAF_LISTA_PRESENCA
                WHERE PROPOSTA_TURMA_ID = @propostaTurmaId
                  AND ID <> @listaPresencaId
                  AND NOT EXCLUIDO
                """;

            var parametros = new
            {
                propostaTurmaId,
                listaPresencaId
            };

            return await conexao.Obter().QueryFirstOrDefaultAsync<bool>(query, parametros);
        }
        public async Task<ResultadoPaginado<ListagemResultadoCodafListaPresencaDto>> ObterListagemResultadoCodafListaPresencaPorFiltroAsync(FiltroListagemResultadoCodafListaPresencaDto filtro)
        {
            const string sqlBaseJoins = """
                FROM   PUBLIC.CODAF_LISTA_PRESENCA AS CLP
                INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
                INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID 
                INNER JOIN PUBLIC.AREA_PROMOTORA AS AP ON P.AREA_PROMOTORA_ID = AP.ID
                """;
            const string sqlBaseOrderBy = """
                ORDER  BY
                        CASE WHEN CLP.DATA_ENVIO_DF IS NULL THEN 0
                             ELSE 1
                        END DESC,
                        CLP.DATA_ENVIO_DF ASC,
                        CLP.CRIADO_EM DESC
                """;

            var condicoesWhere = new StringBuilder("WHERE NOT CLP.EXCLUIDO AND NOT PT.EXCLUIDO AND NOT P.EXCLUIDO ");
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
                condicoesWhere.Append(" AND CLP.STATUS = @status ");
                parametros.Add("status", filtro.Status.Value);
            }

            if (filtro.DataEnvioDf is not null)
            {
                condicoesWhere.Append(" AND DATE(CLP.DATA_ENVIO_DF) = DATE(@dataEnvioDf) ");
                parametros.Add("dataEnvioDf", filtro.DataEnvioDf.Value);
            }

            if (filtro.PerfilRestrito)
            {
                condicoesWhere.Append(" AND CLP.CRIADO_LOGIN = '" + contexto.LoginUsuario + "'");
                parametros.Add("perfilRestrito", filtro.PerfilRestrito);
            }

            var conn = conexao.Obter();
            var sqlCount = new StringBuilder($"""
                SELECT COUNT(1)
                {sqlBaseJoins}
                {condicoesWhere}
                """);

            var totalRegistros = await conn.QueryFirstAsync<int>(sqlCount.ToString(), parametros);
            if (totalRegistros == 0)
                return new ResultadoPaginado<ListagemResultadoCodafListaPresencaDto>
                {
                    Itens = [],
                    PaginaAtual = filtro.Pagina,
                    TamanhoPagina = filtro.TamanhoPagina,
                    TotalRegistros = 0
                };

            var registrosIgnorados = (filtro.Pagina - 1) * filtro.TamanhoPagina;
            parametros.Add("limite", filtro.TamanhoPagina);
            parametros.Add("registrosIgnorados", registrosIgnorados);
            parametros.Add("statusPendente", StatusProcessamentoCertificadoCodaf.Pendente);
            parametros.Add("statusEmProcessamento", StatusProcessamentoCertificadoCodaf.EmProcessamento);
            parametros.Add("statusProcessadoComSucesso", StatusProcessamentoCertificadoCodaf.ProcessadoComSucesso);
            parametros.Add("statusProcessadoComErro", StatusProcessamentoCertificadoCodaf.ProcessadoComErro);

            var sqlConsulta = new StringBuilder($"""
                SELECT CLP.ID,
                       P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                       p.NOME_FORMACAO AS nomeFormacao,
                       p.ID AS codigoFormacao,
                       pt.NOME AS nomeTurma,
                       ap.NOME AS nomeAreaPromotora,
                       CLP.STATUS,
                       CASE 
                	        -- 0: Sem Cetificado
                           WHEN P.CURSO_COM_CERTIFICADO = FALSE THEN 0

                           -- 1: Pendente Emissão
                           WHEN NOT EXISTS (
                               SELECT 1 
                               FROM PUBLIC.CODAF_LOG_REMESSA_CONCLUSAO AS L 
                               WHERE L.CODAF_LISTA_PRESENCA_ID = CLP.ID
                           ) THEN 1

                           -- 3: Em Processamento
                           WHEN EXISTS (
                           	SELECT 1
                           	FROM  PUBLIC.CODAF_CERTIFICADOS AS CC
                           	WHERE CC.CODAF_LISTA_PRESENCA_ID = CLP.ID AND CC.STATUS_PROCESSAMENTO IN (@statusPendente, @statusEmProcessamento)
                           ) THEN 3

                           -- 4: Emitido
                           WHEN EXISTS (
                           	SELECT 1
                           	FROM  PUBLIC.CODAF_CERTIFICADOS AS CC
                           	WHERE CC.CODAF_LISTA_PRESENCA_ID = CLP.ID AND CC.STATUS_PROCESSAMENTO IN (@statusProcessadoComSucesso, @statusProcessadoComErro)
                           ) THEN 4

                           -- 2: Disponível para Emissão
                           ELSE 2
                       END AS statusCertificacaoTurma,
                       CLP.CODIGO_CURSO_EOL codigoCursoEol,
                       CLP.CODIGO_NIVEL codigoNivel
                {sqlBaseJoins}
                {condicoesWhere}
                {sqlBaseOrderBy}
                LIMIT @limite OFFSET @registrosIgnorados
                """);

            var itens = await conn.QueryAsync<ListagemResultadoCodafListaPresencaDto>(sqlConsulta.ToString(), parametros);
            return new ResultadoPaginado<ListagemResultadoCodafListaPresencaDto>
            {
                Itens = itens,
                PaginaAtual = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina,
                TotalRegistros = totalRegistros
            };
        }

        public async Task<CodafListaPresenca?> ObterPorIdDetalhadoAsync(long id)
        {
            var conn = conexao.Obter();
            var sql = $"""
                -- 1. Dados do Cabeçalho (CODAF + Proposta + Turma)
                {sqlObterCodafPorIdComPropostaEPropostaTurma}

                -- 2. Retificações
                {sqlObterRetificacoesPorIdCodaf}

                -- 3. Anexos
                {sqlObterAnexosPorIdCodaf}

                -- 4. Inscritos (A lista grande)
                {sqlObterInscricoesDaListaPorIdCodaf}
                
                -- 5. Critérios de Certificação
                {CodafQueries.SqlObterCriteriosCertificacaoPorIdCodaf}
                """;

            var parametros = new { id };

            using var multi = await conn.QueryMultipleAsync(sql, parametros);
            var codafListaPresenca = multi.Read<CodafListaPresenca, Proposta, PropostaTurma, CodafListaPresenca>(
            (clp, p, pt) =>
            {
                clp.Proposta = p;
                clp.PropostaTurma = pt;
                return clp;
            },
            splitOn: "ID,ID").SingleOrDefault();

            if (codafListaPresenca == null)
                return null;

            codafListaPresenca.CodafRetificacoes = [.. await multi.ReadAsync<CodafRetificacaoListaPresenca>()];
            codafListaPresenca.CodafAnexos = [.. await multi.ReadAsync<CodafAnexo>()];
            codafListaPresenca.CodafInscricoes = [.. await multi.ReadAsync<CodafInscricaoListaPresenca>()];

            if (codafListaPresenca.Proposta == null)
                return codafListaPresenca;

            codafListaPresenca.Proposta.CriterioCertificacao = [.. await multi.ReadAsync<PropostaCriterioCertificacao>()];
            return codafListaPresenca;
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

                const string sqlListaPresenca = """
                    UPDATE PUBLIC.CODAF_LISTA_PRESENCA
                    SET    EXCLUIDO = @Excluido,
                           ALTERADO_EM = @AlteradoEm,
                           ALTERADO_POR = @AlteradoPor,
                           ALTERADO_LOGIN = @AlteradoLogin
                    WHERE  ID = @Id
                    """;
                await conn.ExecuteAsync(sqlListaPresenca, parametrosAtualizacao, transaction);

                const string sqlInscricoes = """
                    UPDATE PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA
                    SET    EXCLUIDO = @Excluido,
                           ALTERADO_EM = @AlteradoEm,
                           ALTERADO_POR = @AlteradoPor,
                           ALTERADO_LOGIN = @AlteradoLogin
                    WHERE  CODAF_LISTA_PRESENCA_ID = @Id and NOT EXCLUIDO
                    """;

                await conn.ExecuteAsync(sqlInscricoes, parametrosAtualizacao, transaction);

                const string sqlAnexos = """
                    UPDATE PUBLIC.CODAF_ANEXO
                    SET    EXCLUIDO = @Excluido,
                           ALTERADO_EM = @AlteradoEm,
                           ALTERADO_POR = @AlteradoPor,
                           ALTERADO_LOGIN = @AlteradoLogin
                    WHERE  CODAF_LISTA_PRESENCA_ID = @Id and NOT EXCLUIDO
                    """;

                await conn.ExecuteAsync(sqlAnexos, parametrosAtualizacao, transaction);

                const string sqlRetificacoes = """
                    UPDATE PUBLIC.CODAF_RETIFICACAO_LISTA_PRESENCA
                    SET    EXCLUIDO = @Excluido,
                           ALTERADO_EM = @AlteradoEm,
                           ALTERADO_POR = @AlteradoPor,
                           ALTERADO_LOGIN = @AlteradoLogin
                    WHERE  CODAF_LISTA_PRESENCA_ID = @Id and NOT EXCLUIDO
                    """;

                await conn.ExecuteAsync(sqlRetificacoes, parametrosAtualizacao, transaction);

                const string sqlComentarios = """
                    UPDATE PUBLIC.CODAF_COMENTARIO_LISTA_PRESENCA
                    SET    EXCLUIDO = @Excluido,
                           ALTERADO_EM = @AlteradoEm,
                           ALTERADO_POR = @AlteradoPor,
                           ALTERADO_LOGIN = @AlteradoLogin
                    WHERE  CODAF_LISTA_PRESENCA_ID = @Id and NOT EXCLUIDO
                    """;

                await conn.ExecuteAsync(sqlComentarios, parametrosAtualizacao, transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<CodafListaPresenca?> ObterPorIdComPropostaEPropostaTurmaAsync(long id)
        {
            var conn = conexao.Obter();
            var parametros = new { id };
            var codafListaPresenca = await conn.QueryAsync<CodafListaPresenca, Proposta, PropostaTurma, CodafListaPresenca>(
                sqlObterCodafPorIdComPropostaEPropostaTurma,
                (clp, p, pt) =>
                {
                    clp.Proposta = p;
                    clp.PropostaTurma = pt;
                    return clp;
                },
                parametros,
                splitOn: "ID,ID");
            return codafListaPresenca.SingleOrDefault();
        }

        public async Task<IEnumerable<DadosConsultaParaTxtEolDto>?> ObterDadosRemessaConclusaoCodafAsync(long id)
        {
            var conn = conexao.Obter();
            const string query = """
                SELECT U.LOGIN registroFuncional,
                       CLP.CODIGO_CURSO_EOL codigoCursoEol,
                       P.DATA_REALIZACAO_FIM dataFimCurso,
                       CLP.CODIGO_NIVEL codigoNivel,
                       P.NUMERO_HOMOLOGACAO numeroHomologacao,
                       P.HORAS_TOTAIS horasTotais,
                       P.CARGA_HORARIA_TOTAL_OUTRA cargaHorariaTotalOutra,
                       PT.NOME nomeTurma
                FROM   PUBLIC.CODAF_LISTA_PRESENCA AS CLP
                       INNER JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA AS CILP  ON CILP.CODAF_LISTA_PRESENCA_ID = CLP.ID
                       INNER JOIN PUBLIC.INSCRICAO AS INSCR ON INSCR.ID = CILP.INSCRICAO_ID
                       INNER JOIN PUBLIC.USUARIO AS U ON U.ID = INSCR.USUARIO_ID
                       INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON PT.ID = CLP.PROPOSTA_TURMA_ID
                       INNER JOIN PUBLIC.PROPOSTA AS P ON P.ID = PT.PROPOSTA_ID
                WHERE NOT CLP.EXCLUIDO AND NOT CILP.EXCLUIDO AND NOT INSCR.EXCLUIDO 
                  AND NOT PT.EXCLUIDO AND NOT P.EXCLUIDO
                  AND CILP.APROVADO
                  AND CLP.ID = @id;
                """;
            var parametros = new { id };
            var resultado = await conn.QueryAsync<DadosConsultaParaTxtEolDto>(query, parametros);
            return resultado;
        }

        private const string sqlObterCodafPorIdComPropostaEPropostaTurma = """
            SELECT CLP.ID,
                   CLP.PROPOSTA_ID AS propostaId,
                   CLP.PROPOSTA_TURMA_ID AS propostaTurmaId,
                   CLP.DATA_PUBLICACAO AS dataPublicacao,
                   CLP.DATA_PUBLICACAO_DOM AS dataPublicacaoDom,
                   CLP.NUMERO_COMUNICADO AS numeroComunicado,
                   CLP.PAGINA_COMUNICADO_DOM AS paginaComunicadoDom,
                   CLP.CODIGO_CURSO_EOL AS codigoCursoEol,
                   CLP.CODIGO_NIVEL AS codigoNivel,
                   CLP.OBSERVACAO,
                   CLP.STATUS,
                   CLP.ALTERADO_EM AS alteradoEm,
                   CLP.ALTERADO_POR AS alteradoPor,
                   CLP.ALTERADO_LOGIN AS alteradoLogin,
                   CLP.CRIADO_EM AS criadoEm,
                   CLP.CRIADO_POR AS criadoPor,
                   CLP.CRIADO_LOGIN AS criadoLogin,
           
                   P.ID, 
                   P.NOME_FORMACAO AS nomeFormacao,
                   P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
           
                   PT.ID, 
                   PT.NOME
            FROM PUBLIC.CODAF_LISTA_PRESENCA AS CLP
            INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
            INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
            WHERE NOT CLP.EXCLUIDO AND NOT PT.EXCLUIDO AND NOT P.EXCLUIDO 
              AND CLP.ID = @id;
            """;

        private const string sqlObterRetificacoesPorIdCodaf = """
            SELECT CRLP.ID, 
                   CRLP.CODAF_LISTA_PRESENCA_ID AS CodafListaPresencaId,
                   CRLP.DATA_RETIFICACAO AS DataRetificacao,
                   CRLP.PAGINA_RETIFICACAO_DOM AS PaginaRetificacaoDom,
                   CRLP.CRIADO_EM AS CriadoEm,
                   CRLP.CRIADO_POR AS CriadoPor
            FROM PUBLIC.CODAF_RETIFICACAO_LISTA_PRESENCA AS CRLP
            WHERE NOT CRLP.EXCLUIDO AND CRLP.CODAF_LISTA_PRESENCA_ID = @id;
            """;

        private const string sqlObterAnexosPorIdCodaf = """
            SELECT CA.ID, 
                   CA.CODAF_LISTA_PRESENCA_ID AS CodafListaPresencaId,
                   CA.ARQUIVO_CODIGO AS ArquivoCodigo,
                   CA.NOME_ARQUIVO AS NomeArquivo,
                   CA.EXTENSAO AS Extensao,
                   CA.TIPO_ANEXO_ID AS TipoAnexoId,
                   CA.CRIADO_EM AS CriadoEm,
                   CA.CRIADO_POR AS CriadoPor
            FROM PUBLIC.CODAF_ANEXO AS CA 
            WHERE NOT CA.EXCLUIDO AND CA.CODAF_LISTA_PRESENCA_ID = @id;
            """;

        private const string sqlObterInscricoesDaListaPorIdCodaf = """
            SELECT CILP.ID, 
                   CILP.CODAF_LISTA_PRESENCA_ID AS CodafListaPresencaId,
                   CILP.INSCRICAO_ID AS InscricaoId,
                   CILP.PERCENTUAL_FREQUENCIA AS PercentualFrequencia,
                   CILP.ATIVIDADE_OBRIGATORIO AS AtividadeObrigatorio,
                   CILP.CONCEITO_FINAL AS ConceitoFinal,
                   CILP.APROVADO AS Aprovado,
                   CILP.CRIADO_EM AS CriadoEm,
                   CILP.CRIADO_POR AS CriadoPor
            FROM PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA AS CILP 
            WHERE NOT CILP.EXCLUIDO AND CILP.CODAF_LISTA_PRESENCA_ID = @id;
            """;        
    }
}