using MimeKit;

namespace SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces
{
    public interface IServicoEnvioEmail
    {
        Task EnviarAsync(MimeMessage mensagem, CancellationToken cancellationToken);
    }
}
