using MediatR;
using MimeKit;
using SME.ConectaFormacao.Dominio.Utilitarios;
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

            var chaveIdempotencia = GeradorChaveIdempotencia.Gerar(
                request.EmailDestinatario,
                request.Assunto,
                comJanelaTemporal: false);

            var resultado = await servicoEnvioEmail.EnviarComIdempotenciaAsync(
                message,
                chaveIdempotencia,
                cancellationToken);

            return resultado.Enviado || resultado.JaEnviado;
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