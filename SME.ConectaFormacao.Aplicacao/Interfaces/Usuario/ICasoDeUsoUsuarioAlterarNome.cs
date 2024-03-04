namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioAlterarNome
    {
        Task<bool> Executar(string login, string nome);
    }
}
