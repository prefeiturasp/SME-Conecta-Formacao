namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioAlterarUnidadeEol
    {
        Task<bool> Executar(string login, string codigoEolUnidade);
    }
}
