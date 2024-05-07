namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterRelatorioPropostaLaudaPublicacao
    {
        Task<string> Executar(long propostaId);
    }
}
