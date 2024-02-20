namespace SME.ConectaFormacao.Infra.Servicos.GitHub.Interfaces
{
    public interface IServicoGithub
    {
        Task<string> RecuperarUltimaVersao();
    }
}
