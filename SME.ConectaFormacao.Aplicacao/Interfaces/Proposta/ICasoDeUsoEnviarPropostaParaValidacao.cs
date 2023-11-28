namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoEnviarPropostaParaValidacao
    {
        Task<bool> Executar(long propostaId);
    }
}