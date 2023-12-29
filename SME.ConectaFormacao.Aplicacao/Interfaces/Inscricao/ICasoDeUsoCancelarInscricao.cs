namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoCancelarInscricao
    {
        Task<bool> Executar(long id);
    }
}
