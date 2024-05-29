using MailKit.Net.Smtp;
using MediatR;
using MimeKit;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarEmailCommandHandler : IRequestHandler<EnviarEmailCommand, bool>
    {
        private readonly IServicoAcessos servicoAcessos;

        public EnviarEmailCommandHandler(IServicoAcessos servicoAcessos)
        {
            this.servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public async Task<bool> Handle(EnviarEmailCommand request, CancellationToken cancellationToken)
        {
            var configuracaoEmail = await servicoAcessos.ObterConfiguracaoEmail();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(configuracaoEmail.Nome, configuracaoEmail.Email));
            message.To.Add(new MailboxAddress(request.NomeDestinatario, request.EmailDestinatario));
            message.Subject = request.Assunto;

            message.Body = new TextPart("html")
            {
                Text = request.MensagemHtml
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(configuracaoEmail.Smtp, configuracaoEmail.Porta, configuracaoEmail.TLS,
                cancellationToken);
            await client.AuthenticateAsync(configuracaoEmail.Usuario, configuracaoEmail.Senha, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            return true;
        }
    }
}