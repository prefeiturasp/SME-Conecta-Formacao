using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarEmailCancelarInscricaoCommandHandler(IRepositorioInscricao repositorioInscricao,
        IMediator mediator) : IRequestHandler<EnviarEmailCancelarInscricaoCommand, bool>
    {
        public async Task<bool> Handle(EnviarEmailCancelarInscricaoCommand request, CancellationToken cancellationToken)
        {
            var dadosParaEmail = await repositorioInscricao.ObterDadosInscricaoPorInscricaoId(request.InscricaoId);
            if (dadosParaEmail == null || !dadosParaEmail.Any())
                return true;

            if (string.IsNullOrWhiteSpace(dadosParaEmail.First().Email))
                return true;

            var destinatario = new EnviarEmailDto
            {
                EmailDestinatario = dadosParaEmail.First().Email,
                NomeDestinatario = dadosParaEmail.First().NomeDestinatario,
                Titulo = $"Cancelamento de inscrição | Formação {dadosParaEmail.First().NomeFormacao} ",
                Texto = CriarMensagemEmail(dadosParaEmail.First().NomeFormacao, request.Motivo)
            };
            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, destinatario), cancellationToken);

            return true;
        }

        private static string CriarMensagemEmail(string nomeFormacao, string? motivoCancelamento)
        {
            var mensagem = new StringBuilder("""
                <!DOCTYPE html>                
                <html lang="pt-BR">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
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
                    <div class="container">
                        <div class="header">Cancelamento de Inscrição</div>
            """);
            mensagem.AppendLine($"<p>A sua inscrição na formação {nomeFormacao} foi cancelada.</p>");

            if (!string.IsNullOrWhiteSpace(motivoCancelamento))
                mensagem.AppendLine(@$" <p>Motivo: {motivoCancelamento}</p>");

            mensagem.AppendLine(
                """
                            <div class="footer">
                                <p>Acesse a nossa <a href="https://conectaformacao.sme.prefeitura.sp.gov.br/area-publica" class="link">área pública</a> e fique por dentro de todas as formações e eventos.</p>
                            </div>
                        </div>
                    </body>
                </html>
                """);


            return mensagem.ToString();
        }
    }
}