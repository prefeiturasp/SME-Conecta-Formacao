namespace SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora
{
    public interface ICasoDeUsoRemoverAreaPromotora
    {
        Task<bool> Executar(long id);
    }
}
