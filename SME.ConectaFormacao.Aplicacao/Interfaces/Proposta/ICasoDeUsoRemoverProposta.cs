namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoRemoverProposta
    {
        Task<bool> Executar(long id);
    }
}
