using Dapper;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioAtribuicaoAulaServidor(IConectaFormacaoConexao conexao) : IRepositorioAtribuicaoAulaServidor
    {
        public async Task<DateTime?> ObterDataUltimaAtualizacaoAsync()
        {
            const string query = @"
                SELECT MAX(data_atualizacao)
                FROM Atribuicoes_Servidor_Eol";

            var dataUtc = await conexao.Obter().QueryFirstOrDefaultAsync<DateTime?>(query);
            return dataUtc?.ToLocalTime();
        }
    }
}
