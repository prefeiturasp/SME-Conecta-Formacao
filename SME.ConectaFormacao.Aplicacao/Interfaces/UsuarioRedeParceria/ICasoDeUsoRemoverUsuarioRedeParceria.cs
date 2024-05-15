namespace SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria
{
    public interface ICasoDeUsoRemoverUsuarioRedeParceria
    {
        Task<bool> Executar(long id);
    }
}
