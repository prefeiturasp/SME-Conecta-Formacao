namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioAtribuicaoAulaServidor
    {
        Task<DateTime?> ObterDataUltimaAtualizacaoAsync();
    }
}