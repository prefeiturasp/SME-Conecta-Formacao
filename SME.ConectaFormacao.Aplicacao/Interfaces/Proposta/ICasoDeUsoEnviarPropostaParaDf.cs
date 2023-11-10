namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoEnviarPropostaParaDf
    {
        Task<bool> Executar(long propostaId);
    }
}