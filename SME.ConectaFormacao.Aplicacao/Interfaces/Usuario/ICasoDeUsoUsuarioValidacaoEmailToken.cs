namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioValidacaoEmailToken
    {
        Task<bool> Executar(Guid token);
    }
}
