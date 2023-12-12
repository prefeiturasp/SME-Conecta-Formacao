using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.TesteIntegracao.ServicosFakes
{
    internal class CacheDistribuidoFaker : ICacheDistribuido
    {
        public Task<string> ObterAsync(string nomeChave, bool utilizarGZip = false)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<T> ObterAsync<T>(string nomeChave, Func<Task<T>> buscarDados, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            return buscarDados();
        }

        public Task<T> ObterObjetoAsync<T>(string nomeChave, bool utilizarGZip = false) where T : new()
        {
            return default;
        }

        public Task RemoverAsync(string nomeChave)
        {
            return Task.CompletedTask;
        }

        public Task SalvarAsync(string nomeChave, string valor, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            return Task.CompletedTask;
        }
    }
}
