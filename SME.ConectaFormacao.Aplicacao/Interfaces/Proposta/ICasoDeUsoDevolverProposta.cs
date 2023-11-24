namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoDevolverProposta
    {
        Task<bool> Executar(long propostaId, string justificativa);
    }
}