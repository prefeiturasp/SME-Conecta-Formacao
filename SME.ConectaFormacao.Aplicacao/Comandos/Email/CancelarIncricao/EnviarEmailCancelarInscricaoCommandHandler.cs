using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarEmailCancelarInscricaoCommandHandler : IRequestHandler<EnviarEmailCancelarInscricaoCommand, bool>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMediator _mediator;

        public EnviarEmailCancelarInscricaoCommandHandler(IRepositorioInscricao repositorioInscricao,
            IMediator mediator)
        {
            _repositorioInscricao =
                repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        public async Task<bool> Handle(EnviarEmailCancelarInscricaoCommand request, CancellationToken cancellationToken)
        {
            var dadosParaEmail = await _repositorioInscricao.ObterDadasInscricaoPorInscricaoId(request.InscricaoId);
            var destinatario = new EnviarEmailDto
            {
                EmailDestinatario = dadosParaEmail.FirstOrDefault()!.Email,
                NomeDestinatario = dadosParaEmail.FirstOrDefault()!.NomeDestinatario,
                Titulo = $"Cancelamento de inscrição | Formação {dadosParaEmail.FirstOrDefault()!.NomeFormacao} ",
                Texto = CriarMensagemEmail(dadosParaEmail.FirstOrDefault()!.NomeFormacao, request.Motivo)
            };
            await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, destinatario), cancellationToken);

            return true;
        }

        private string CriarMensagemEmail(string nomeFormacao, string motivoCancelamento)
        {
            var mensagem = new StringBuilder(@"<!DOCTYPE html>
                                            <html lang=""pt-BR"">
                                            <head>
                                                <meta charset=""UTF-8"">
                                                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                                <title>Cancelamento de Inscrição</title>
                                                <style>
                                                    body {
                                                        font-family: Arial, sans-serif;
                                                        margin: 20px;
                                                        line-height: 1.6;
                                                    }
                                                    .container {
                                                        background-color: #fff;
                                                        padding: 20px;
                                                        border-radius: 8px;
                                                    }
                                                    .header {
                                                        font-size: 20px;
                                                        font-weight: bold;
                                                        margin-bottom: 10px;
                                                    }                                                   
                                                    .footer {
                                                        margin-top: 20px;
                                                    }
                                                    .link {
                                                        color: #337ab7;
                                                        text-decoration: none;
                                                    }
                                                </style>
                                            </head>
                                            <body>
                                                <div class=""container"">
                                                    <div class=""header"">Cancelamento de Inscrição</div>");
            mensagem.AppendLine($"<p>A sua inscrição na formação {nomeFormacao} foi cancelada.</p>");

            if (motivoCancelamento.EstaPreenchido())
                mensagem.AppendLine(@$" <p>Motivo: {motivoCancelamento}.</p>");

            mensagem.AppendLine(@"<div class=""footer"">
                                    <p>Acesse a nossa <a href=""https://conectaformacao.sme.prefeitura.sp.gov.br/area-publica"" class=""link"">área pública</a> e fique por dentro de todas as formações e eventos.</p>
                                  </div>
                                  </div>
                                  </body>
                                  </html>");


            return mensagem.ToString();

        }
    }
}