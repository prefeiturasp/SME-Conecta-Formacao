namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioValidacaoToken
    {
        Task<bool> Executar(Guid token);
    }
}
