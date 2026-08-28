using MediatR;
using MimeKit;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.Utilitarios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Enviar.EnviarEmail
{
    public class EnviarEmailCommandHandler(
        IServicoEnvioEmail servicoEnvioEmail,
        IServicoAcessos servicoAcessos,
        IRepositorioEmailEnviado repositorioEmailEnviado) : IRequestHandler<EnviarEmailCommand, bool>
    {
        public async Task<bool> Handle(EnviarEmailCommand request, CancellationToken cancellationToken)
        {
            var configuracaoEmail = await servicoAcessos.ObterConfiguracaoEmail();
            var message = MontarMensagem(request, configuracaoEmail);

            var chaveIdempotencia = GeradorChaveIdempotencia.Gerar(
                request.EmailDestinatario,
                request.Assunto,
                comJanelaTemporal: false);

            if (await repositorioEmailEnviado.ExistePorChaveIdempotenciaAsync(chaveIdempotencia))
                return true;

            var resultado = await servicoEnvioEmail.EnviarComIdempotenciaAsync(
                message,
                chaveIdempotencia,
                cancellationToken);

            var enviado = resultado.Enviado || resultado.JaEnviado;
            await repositorioEmailEnviado.Inserir(new EmailEnviado
            {
                ChaveIdempotencia = chaveIdempotencia,
                EnviadoEm = DateTime.Now,
                MensagemErro = resultado.MensagemErro,
                CriadoEm = DateTime.Now,
                EmailDestinatario = request.EmailDestinatario,
                ConteudoHash = request.MensagemHtml.GerarHashSHA256(),
                NomeDestinatario = request.NomeDestinatario,
                Titulo = request.Assunto,
                Enviado = enviado,
            });

            return enviado;
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