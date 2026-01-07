using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
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
                             ELSE 0
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

            var sqlConsulta = new StringBuilder($"""
                SELECT CLP.ID,
                       P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                       p.NOME_FORMACAO AS nomeFormacao,
                       p.ID AS codigoFormacao,
                       pt.NOME AS nomeTurma,
                       ap.NOME AS nomeAreaPromotora,
                       CLP.STATUS,
                       CASE WHEN NOT P.CURSO_COM_CERTIFICADO THEN 0
                            ELSE 1 END AS statusCertificacaoTurma,
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
                SELECT CLP.ID,
                       CLP.PROPOSTA_ID AS propostaId,
                       CLP.PROPOSTA_TURMA_ID AS propostaTurmaId,
                       CLP.DATA_PUBLICACAO  AS dataPublicacao,
                       CLP.DATA_PUBLICACAO_DOM  AS dataPublicacaoDom,
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
                       P.ID, --Split de proposta 
                       P.NOME_FORMACAO AS nomeFormacao,
                       P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                       PT.ID, --Split de proposta turma
                       PT.NOME,

                       CRLP.ID, -- Split 3
                       CRLP.CODAF_LISTA_PRESENCA_ID AS CodafListaPresencaId,
                       CRLP.DATA_RETIFICACAO AS DataRetificacao,
                       CRLP.PAGINA_RETIFICACAO_DOM AS PaginaRetificacaoDom,
                       CRLP.ALTERADO_EM AS AlteradoEm,
                       CRLP.ALTERADO_POR AS AlteradoPor,
                       CRLP.ALTERADO_LOGIN AS AlteradoLogin,
                       CRLP.CRIADO_EM AS CriadoEm,
                       CRLP.CRIADO_POR AS CriadoPor,
                       CRLP.CRIADO_LOGIN AS CriadoLogin,

                       CA.ID, -- Split 4 (Anexos)
                       CA.CODAF_LISTA_PRESENCA_ID AS CodafListaPresencaId,
                       CA.ARQUIVO_CODIGO AS ArquivoCodigo,
                       CA.NOME_ARQUIVO AS NomeArquivo,
                       CA.EXTENSAO AS Extensao,
                       CA.TIPO_ANEXO_ID AS TipoAnexoId,
                       CA.ALTERADO_EM AS AlteradoEm,
                       CA.ALTERADO_POR AS AlteradoPor,
                       CA.ALTERADO_LOGIN AS AlteradoLogin,
                       CA.CRIADO_EM AS CriadoEm,
                       CA.CRIADO_POR AS CriadoPor,
                       CA.CRIADO_LOGIN AS CriadoLogin
                FROM PUBLIC.CODAF_LISTA_PRESENCA AS CLP
                INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
                INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
                LEFT JOIN PUBLIC.CODAF_RETIFICACAO_LISTA_PRESENCA AS CRLP ON CRLP.CODAF_LISTA_PRESENCA_ID = CLP.ID AND NOT CRLP.EXCLUIDO
                LEFT JOIN PUBLIC.CODAF_ANEXO AS CA ON CA.CODAF_LISTA_PRESENCA_ID = CLP.ID AND NOT CA.EXCLUIDO
                WHERE NOT CLP.EXCLUIDO AND NOT PT.EXCLUIDO AND NOT P.EXCLUIDO AND CLP.ID = @id
                """;

            var parametros = new { id }; 
            var listaPresencaDict = new Dictionary<long, CodafListaPresenca>();
            await conn.QueryAsync<CodafListaPresenca, Proposta, PropostaTurma, CodafRetificacaoListaPresenca, CodafAnexo, CodafListaPresenca>(
                sql,
                (clp, p, pt, crlp, ca) =>
                {
                    if (!listaPresencaDict.TryGetValue(clp.Id, out var listaPresencaEntry))
                    {
                        listaPresencaEntry = clp;
                        listaPresencaEntry.Proposta = p;
                        listaPresencaEntry.PropostaTurma = pt;
                        listaPresencaEntry.CodafRetificacoes = [];
                        listaPresencaEntry.CodafAnexos = [];
                        listaPresencaDict.Add(listaPresencaEntry.Id, listaPresencaEntry);
                    }

                    if (crlp != null)
                    {
                        listaPresencaEntry.CodafRetificacoes.Add(crlp);
                    }

                    if (ca != null)
                    {
                        listaPresencaEntry.CodafAnexos!.Add(ca);
                    }

                    return listaPresencaEntry;
                },
                parametros,
                splitOn: "ID,ID,ID,ID");
            return listaPresencaDict.Values.FirstOrDefault();
        }
    }
}