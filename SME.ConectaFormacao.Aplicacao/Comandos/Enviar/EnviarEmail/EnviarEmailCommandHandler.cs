using MediatR;
using MimeKit;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Enviar.EnviarEmail
{
    public class EnviarEmailCommandHandler(
        IServicoEnvioEmail servicoEnvioEmail,
        IServicoAcessos servicoAcessos) : IRequestHandler<EnviarEmailCommand, bool>
    {
        public async Task<bool> Handle(EnviarEmailCommand request, CancellationToken cancellationToken)
        {
            var configuracaoEmail = await servicoAcessos.ObterConfiguracaoEmail();
            var message = MontarMensagem(request, configuracaoEmail);
            await servicoEnvioEmail.EnviarAsync(message, cancellationToken);
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