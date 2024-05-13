namespace SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces
{
    public interface IServicoRelatorio
    {
        Task<string> ObterRelatorioPropostaLaudaDePublicacao(long propostaId);
        Task<string> ObterRelatorioPropostaLaudaCompleta(long propostaId);
    }
}
