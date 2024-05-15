namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoEnviarPropostaParecerista
    {
        Task<bool> Executar(long propostaId);
    }
}
