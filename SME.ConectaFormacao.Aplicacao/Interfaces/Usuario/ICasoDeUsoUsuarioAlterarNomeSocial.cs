namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioAlterarNomeSocial
    {
        Task<bool> Executar(string login, string? nome);
    }
}
