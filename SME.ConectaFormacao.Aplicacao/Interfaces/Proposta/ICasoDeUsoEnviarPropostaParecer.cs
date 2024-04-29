namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoEnviarPropostaParecer
    {
        Task<bool> Executar(long propostaId);
    }
}
