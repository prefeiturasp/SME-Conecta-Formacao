namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoReenviarEmail
    {
        Task<bool> Executar(string login);
    }
}
