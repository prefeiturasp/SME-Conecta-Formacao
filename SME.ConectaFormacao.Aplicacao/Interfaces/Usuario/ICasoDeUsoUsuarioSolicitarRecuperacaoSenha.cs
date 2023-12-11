namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioSolicitarRecuperacaoSenha
    {
        Task<string> Executar(string login);
    }
}
