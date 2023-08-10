namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioAlterarEmail
    {
        Task<bool> Executar(string login, string email);
    }
}
