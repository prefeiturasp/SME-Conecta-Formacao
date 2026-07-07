using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafSuplementarRetificacao(IContextoAplicacao contexto, IConectaFormacaoConexao conectaFormacaoConexao) :
        RepositorioBaseAuditavel<CodafSuplementarRetificacao>(contexto, conectaFormacaoConexao),
        IRepositorioCodafSuplementarRetificacao
    {
        public async Task<IEnumerable<CodafSuplementarRetificacao>> ObterPorCodafSuplementarIdAsync(long codafSuplementarId)
        {
            const string sql =
                """
                SELECT *
                FROM PUBLIC.CODAF_SUPLEMENTAR_RETIFICACAO AS CSR
                WHERE CSR.CODAF_SUPLEMENTAR_ID = @codafSuplementarId AND NOT EXCLUIDO
                ORDER BY Id DESC
                """;

            var conn = conexao.Obter();
            var parametros = new { codafSuplementarId };
            return await conn.QueryAsync<CodafSuplementarRetificacao>(sql, parametros);
        }
    }
}