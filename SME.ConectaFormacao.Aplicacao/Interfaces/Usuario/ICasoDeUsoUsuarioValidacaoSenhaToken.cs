namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioValidacaoSenhaToken
    {
        Task<bool> Executar(Guid token);
    }
}
