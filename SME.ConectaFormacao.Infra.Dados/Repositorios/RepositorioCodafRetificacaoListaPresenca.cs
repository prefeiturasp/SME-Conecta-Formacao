using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafRetificacaoListaPresenca(IContextoAplicacao contexto, IConectaFormacaoConexao conectaFormacaoConexao) :
        RepositorioBaseAuditavel<CodafRetificacaoListaPresenca>(contexto, conectaFormacaoConexao),
        IRepositorioCodafRetificacaoListaPresenca
    {
        public async Task<IEnumerable<CodafRetificacaoListaPresenca>> ObterPorListaPresencaIdAsync(long codafListaPresencaId)
        {
            const string sql =
                """
                SELECT *
                FROM PUBLIC.CODAF_RETIFICACAO_LISTA_PRESENCA AS CRLP 
                WHERE CRLP.CODAF_LISTA_PRESENCA_ID = @codafListaPresencaId AND NOT EXCLUIDO
                ORDER BY Id DESC
                """;

            var conn = conexao.Obter();
            var parametros = new { codafListaPresencaId };
            return await conn.QueryAsync<CodafRetificacaoListaPresenca>(sql, parametros);
        }
    }
}