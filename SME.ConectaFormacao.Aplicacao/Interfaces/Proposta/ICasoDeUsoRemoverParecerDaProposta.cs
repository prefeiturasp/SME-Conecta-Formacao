namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoRemoverParecerDaProposta
    {
        Task<bool> Executar(long parecerId);
    }
}