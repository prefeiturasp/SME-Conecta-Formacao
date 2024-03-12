namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoAlterarEmailEReenviarEmailParaValidacao
    {
        Task<bool> Executar(string login, string email);
    }
}