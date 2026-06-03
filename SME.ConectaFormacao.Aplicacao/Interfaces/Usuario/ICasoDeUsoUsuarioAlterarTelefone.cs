namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioAlterarTelefone
    {
        Task<bool> Executar(string login, string nome);
    }
}
