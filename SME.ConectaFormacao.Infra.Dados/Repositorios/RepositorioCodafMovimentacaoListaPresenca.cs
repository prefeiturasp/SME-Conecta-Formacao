using Dapper;
using Dommel;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafMovimentacaoListaPresenca(IConectaFormacaoConexao conexao, IContextoAplicacao contexto)
        : IRepositorioCodafMovimentacaoListaPresenca
    {
        public async Task<long> InserirAsync(CodafMovimentacaoListaPresenca codafMovimentacaoListaPresenca)
        {
            codafMovimentacaoListaPresenca.CriadoLogin = contexto.UsuarioLogado;
            codafMovimentacaoListaPresenca.CriadoEm = DateTimeExtension.HorarioBrasilia();
            codafMovimentacaoListaPresenca.CriadoPor = contexto.NomeUsuario;
            return (long)await conexao.Obter().InsertAsync(codafMovimentacaoListaPresenca);
        }

        public async Task<CodafMovimentacaoListaPresenca?> ObterUltimaMovimentacaoPorListaPresencaIdAsync(long codafListaPresencaId)
        {
            const string query =
                """
                SELECT id,
                       codaf_lista_presenca_id as codafListaPresencaId,
                       status_codaf_lista_presenca as statusCodafListaPresenca,
                       codaf_comentario_lista_presenca_id as codafComentarioListaPresencaId,
                       criado_em as criadoEm,
                       criado_login as criadoLogin,
                       criado_por as criadoPor
                FROM codaf_movimentacao_lista_presenca cmlp
                WHERE cmlp.codaf_lista_presenca_id = @codafListaPresencaId
                ORDER BY id DESC
                LIMIT 1;
                """;
            return await conexao.Obter().QuerySingleOrDefaultAsync<CodafMovimentacaoListaPresenca>(query, new { codafListaPresencaId });
        }

        public async Task<CodafMovimentacaoListaPresenca?> ObterUltimaMovimentacaoPorListaPresencaStatusAsync(long codafListaPresencaId, StatusCodafListaPresenca status)
        {
            const string query =
                """
                SELECT id,
                       codaf_lista_presenca_id as codafListaPresencaId,
                       status_codaf_lista_presenca as statusCodafListaPresenca,
                       codaf_comentario_lista_presenca_id as codafComentarioListaPresencaId,
                       criado_em as criadoEm,
                       criado_login as criadoLogin,
                       criado_por as criadoPor
                FROM codaf_movimentacao_lista_presenca cmlp
                WHERE cmlp.codaf_lista_presenca_id = @codafListaPresencaId
                  AND cmlp.status_codaf_lista_presenca = @status
                ORDER BY id DESC
                LIMIT 1;
                """;
            return await conexao.Obter().QuerySingleOrDefaultAsync<CodafMovimentacaoListaPresenca>(query, new { codafListaPresencaId, status });
        }

        public async Task<CodafMovimentacaoListaPresenca?> ObterPrimeiraMovimentacaoPorListaPresencaStatusAsync(long codafListaPresencaId, StatusCodafListaPresenca status)
        {
            const string query =
                """
                SELECT id,
                       codaf_lista_presenca_id as codafListaPresencaId,
                       status_codaf_lista_presenca as statusCodafListaPresenca,
                       codaf_comentario_lista_presenca_id as codafComentarioListaPresencaId,
                       criado_em as criadoEm,
                       criado_login as criadoLogin,
                       criado_por as criadoPor
                FROM codaf_movimentacao_lista_presenca cmlp
                WHERE cmlp.codaf_lista_presenca_id = @codafListaPresencaId
                  AND cmlp.status_codaf_lista_presenca = @status
                ORDER BY id ASC
                LIMIT 1;
                """;
            return await conexao.Obter().QuerySingleOrDefaultAsync<CodafMovimentacaoListaPresenca>(query, new { codafListaPresencaId, status });
        }
    }
}
