namespace SME.ConectaFormacao.Aplicacao.Integracoes.Interfaces
{
    public interface IServicoGithub
    {
        Task<string> RecuperarUltimaVersao();

    }
}