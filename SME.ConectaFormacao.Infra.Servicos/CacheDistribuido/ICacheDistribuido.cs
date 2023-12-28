namespace SME.ConectaFormacao.Infra.Servicos.Cache
{
    public interface ICacheDistribuido
    {
        Task<string> ObterAsync(string nomeChave, bool utilizarGZip = false);
        Task<T> ObterAsync<T>(string nomeChave, Func<Task<T>> buscarDados, int minutosParaExpirar = 720, bool utilizarGZip = false);
        Task<T> ObterObjetoAsync<T>(string nomeChave, bool utilizarGZip = false) where T : new();
        Task RemoverAsync(string nomeChave);
        Task SalvarAsync(string nomeChave, string valor, int minutosParaExpirar = 720, bool utilizarGZip = false);
        Task SalvarAsync<T>(string nomeChave, T valor, int minutosParaExpirar = 720, bool utilizarGZip = false);
    }
}
