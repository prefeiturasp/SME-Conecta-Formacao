namespace SME.ConectaFormacao.Aplicacao
{
    public interface ICasoDeUsoUsuarioAlterarEmailEducacional
    {
        Task<bool> Executar(string login, string email);
    }
}