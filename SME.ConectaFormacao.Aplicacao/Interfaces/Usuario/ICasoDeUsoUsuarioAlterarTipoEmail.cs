namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioAlterarTipoEmail
    {
        Task<bool> Executar(string login, int tipo);
    }
}
