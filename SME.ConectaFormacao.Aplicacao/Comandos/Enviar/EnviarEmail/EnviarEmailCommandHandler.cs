using MailKit.Net.Smtp;
using MediatR;
using MimeKit;
using Polly;
using Polly.Retry;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using System.Net.Sockets;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Enviar.EnviarEmail
{
    public class EnviarEmailCommandHandler(IServicoAcessos servicoAcessos) : IRequestHandler<EnviarEmailCommand, bool>
    {
        // Define uma política de retry: 3 tentativas com espera exponencial (2s, 4s, 8s)
        private static readonly AsyncRetryPolicy _retryPolicy = Policy
            .Handle<SmtpCommandException>(ex => ex.StatusCode == SmtpStatusCode.TransactionFailed || ex.ErrorCode == SmtpErrorCode.UnexpectedStatusCode)
            .Or<SocketException>()
            .Or<IOException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"Tentativa {retryCount} falhou. Aguardando {timeSpan}...");
                });

        public async Task<bool> Handle(EnviarEmailCommand request, CancellationToken cancellationToken)
        {
            var configuracaoEmail = await servicoAcessos.ObterConfiguracaoEmail();

            var message = MontarMensagem(request, configuracaoEmail);

            await _retryPolicy.ExecuteAsync(async (token) =>
            {
                using var client = new SmtpClient();

                try
                {
                    await client.ConnectAsync(configuracaoEmail.Smtp, configuracaoEmail.Porta, configuracaoEmail.TLS,
                        token);
                    await client.AuthenticateAsync(configuracaoEmail.Usuario, configuracaoEmail.Senha, token);
                    await client.SendAsync(message, token);
                    await client.DisconnectAsync(true, token);
                }
                catch (Exception ex)
                {
                    if (client.IsConnected)
                        await client.DisconnectAsync(false, token);
                    throw;
                }

            }, cancellationToken);
            return true;
        }

        private static MimeMessage MontarMensagem(EnviarEmailCommand request, dynamic configuracaoEmail)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(configuracaoEmail.Nome, configuracaoEmail.Email));
            message.To.Add(new MailboxAddress(request.NomeDestinatario, request.EmailDestinatario));
            message.Subject = request.Assunto;

            var builder = new BodyBuilder
            {
                HtmlBody = request.MensagemHtml
            };

            message.Body = builder.ToMessageBody();
            return message;
        }
    }
}