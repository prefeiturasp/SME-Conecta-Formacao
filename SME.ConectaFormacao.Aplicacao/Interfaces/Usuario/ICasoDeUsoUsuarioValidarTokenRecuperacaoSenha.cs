namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioValidarTokenRecuperacaoSenha
    {
        Task<bool> Executar(Guid token);
    }
}
