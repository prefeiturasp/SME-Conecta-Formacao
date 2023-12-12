namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoEnviarProposta
    {
        Task<bool> Executar(long propostaId);
    }
}