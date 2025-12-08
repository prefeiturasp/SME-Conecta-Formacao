namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoCancelarInscricao
    {
        Task<bool> Executar(long id);
    }
}
