namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterRelatorioPropostaLaudaCompleta
    {
        Task<string> Executar(long propostaId);
    }
}
