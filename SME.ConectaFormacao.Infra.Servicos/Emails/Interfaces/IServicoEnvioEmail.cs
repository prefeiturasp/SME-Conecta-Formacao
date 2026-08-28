using MimeKit;
using SME.ConectaFormacao.Dominio.Dtos;

namespace SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces
{
    public interface IServicoEnvioEmail
    {
        Task EnviarAsync(MimeMessage mensagem, CancellationToken cancellationToken);

        Task<ResultadoEnvioEmail> EnviarComIdempotenciaAsync(
            MimeMessage mensagem,
            string chaveIdempotencia,
            CancellationToken cancellationToken = default);
    }
}
