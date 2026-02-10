using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafComentarioListaPresenca(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) :
        RepositorioBaseAuditavel<CodafComentarioListaPresenca>(contexto, conexao), IRepositorioCodafComentarioListaPresenca
    {
        public async Task<CodafComentarioDevolucaoDto?> ObterUltimoComentarioDevolucaoPorUsuarioAsync(long codafListaPresencaId, StatusCodafListaPresenca statusDevolucao, StatusCodafListaPresenca statusEnvio)
        {
            const string query =
                """
                WITH UltimaDevolucao AS (
                    -- 1. Busca a devolução (Status X) mais recente para este CODAF
                    SELECT ID, 
                           CODAF_COMENTARIO_LISTA_PRESENCA_ID, 
                           CRIADO_POR
                    FROM PUBLIC.CODAF_MOVIMENTACAO_LISTA_PRESENCA
                    WHERE CODAF_LISTA_PRESENCA_ID = @codafListaPresencaId
                      AND STATUS_CODAF_LISTA_PRESENCA = @statusDevolucao
                    ORDER BY ID DESC
                    LIMIT 1
                ),
                UltimoEnvioAnterior AS (
                    -- 2. Busca o envio (Status Y) mais recente que seja ESTRITAMENTE ANTERIOR à devolução encontrada acima
                    SELECT CRIADO_LOGIN
                    FROM PUBLIC.CODAF_MOVIMENTACAO_LISTA_PRESENCA
                    WHERE CODAF_LISTA_PRESENCA_ID = @codafListaPresencaId
                      AND STATUS_CODAF_LISTA_PRESENCA = @statusEnvio
                      AND ID < (SELECT ID FROM UltimaDevolucao) -- Garante a cronologia
                    ORDER BY ID DESC
                    LIMIT 1
                )
                SELECT CCLP.ID,
                       CCLP.CODAF_LISTA_PRESENCA_ID as codafListaPresencaId,
                       CCLP.COMENTARIO,
                       CCLP.CRIADO_POR AS criadoPor,
                       CCLP.CRIADO_LOGIN AS criadoLogin,
                       CCLP.CRIADO_EM AS criadoEm
                FROM PUBLIC.CODAF_COMENTARIO_LISTA_PRESENCA AS CCLP
                -- Garante que pegamos os dados da movimentação de devolução correta
                INNER JOIN UltimaDevolucao UD ON UD.CODAF_COMENTARIO_LISTA_PRESENCA_ID = CCLP.ID
                -- O Join aqui funciona como um filtro: Se o login do envio não bater, a query não retorna nada
                INNER JOIN UltimoEnvioAnterior UEA ON UEA.CRIADO_LOGIN = @login
                WHERE NOT CCLP.EXCLUIDO;
                """;
            return await conexao.Obter().QuerySingleOrDefaultAsync<CodafComentarioDevolucaoDto>(query, new
            {
                codafListaPresencaId,
                login = contexto.UsuarioLogado,
                statusDevolucao,
                statusEnvio
            });
        }
    }
}
