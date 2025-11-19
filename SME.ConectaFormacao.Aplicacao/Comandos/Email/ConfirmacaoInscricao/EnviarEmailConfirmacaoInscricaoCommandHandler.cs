using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarEmailConfirmacaoInscricaoCommandHandler : IRequestHandler<EnviarEmailConfirmacaoInscricaoCommand, bool>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMediator _mediator;

        public EnviarEmailConfirmacaoInscricaoCommandHandler(IRepositorioInscricao repositorioInscricao, IMediator mediator)
        {
            _repositorioInscricao =
                repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mediator =
                mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(EnviarEmailConfirmacaoInscricaoCommand request, CancellationToken cancellationToken)
        {
            var dadosParaEmail = await _repositorioInscricao.ObterDadosInscricaoPorInscricaoId(request.InscricaoId);

            var agrupadoPorUsuario = dadosParaEmail.GroupBy(x => x.UsuarioId);
            foreach (var usuario in agrupadoPorUsuario)
            {
                if (usuario.FirstOrDefault()!.Email.EstaPreenchido())
                {
                    var destinatario = new EnviarEmailDto
                    {
                        EmailDestinatario = usuario.FirstOrDefault()!.Email,
                        NomeDestinatario = usuario.FirstOrDefault()!.NomeDestinatario,
                        Titulo = $"Confirmação de inscrição | {usuario.FirstOrDefault()!.NomeFormacao} ",
                        Texto = CriarTextoEmail(usuario)
                    };
                    await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, destinatario), cancellationToken);
                }
            }

            return true;
        }


        private string CriarTextoEmail(IGrouping<long, InscricaoDadosEmailConfirmacao> usuario)
        {
            var comSga = usuario.Where(x => x.IntegradoSga);
            var semSga = usuario.Where(x => !x.IntegradoSga);
            var texto = new StringBuilder(@"<!DOCTYPE html>
                                                <html lang=""pt-BR"">
                                                <head>
                                                    <meta charset=""UTF-8"">
                                                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                                    <style>
                                                        body {
                                                            font-family: Arial, sans-serif;
                                                            margin: 20px;
                                                            line-height: 1.6;
                                                        }
                                                        h1 {
                                                            color: #333;
                                                        }
                                                        p {
                                                            margin: 10px 0;
                                                        }
                                                        .section {
                                                            margin-bottom: 20px;
                                                        }
                                                        table {
                                                            width: 100%;
                                                            border-collapse: collapse;
                                                            margin: 10px 0;
                                                        }
                                                        table, th, td {
                                                            border: 1px solid #ddd;
                                                        }
                                                        th, td {
                                                            padding: 8px;
                                                            text-align: left;
                                                        }
                                                        th {
                                                            background-color: #f2f2f2;
                                                        }
                                                    </style>
                                                </head>
                                                <body>");

            if (comSga.Any())
            {
                texto.AppendLine(@$"
                <div class=""section"" id=""com-sga"">
                <p>A sua inscrição na formação {usuario.FirstOrDefault().NomeFormacao} foi confirmada. Na data de início da sua turma acesse o SGA para iniciar a formação.</p>
                <p>As aulas irão ocorrer nas seguintes datas:</p>
                <table>
                    <tr>
                        <th>Data</th>
                        <th>Hora</th>
                        <th>Local</th>
                    </tr> ");
                foreach (var cSga in comSga)
                {
                    var data = cSga.DataFim.NaoEhNulo() ? $" {cSga.DataInicio:dd/MM/yyyy} até {cSga.DataFim:dd/MM/yyyy}" :$"{cSga.DataInicio:dd/MM/yyyy}";
                    texto.AppendLine(@$"<tr>
                                        <td>{data}</td>
                                        <td>{cSga.HoraInicio} até {cSga.HoraFim}</td>
                                        <td>{cSga.Local}</td>
                                    </tr>");
                }

                texto.AppendLine(@"</table>
                                    </div>");
            }

            texto.AppendLine("");
            if (semSga.Any())
            {
                texto.AppendLine(@$"    <div class=""section"" id=""sem-sga"">
                                    <p>Sua inscrição foi confirmada. A sua inscrição na formação {usuario.FirstOrDefault().NomeFormacao}.</p>
                                    <p>As aulas irão ocorrer nas seguintes datas:</p>
                                    <table>
                                        <tr>
                                            <th>Data</th>
                                            <th>Hora</th>
                                            <th>Local</th>
                                        </tr>");
                foreach (var sSga in semSga)
                {
                    var data = sSga.DataFim != null ? $" {sSga.DataInicio:dd/MM/yyyy} até {sSga.DataInicio:dd/MM/yyyy}" : $"{sSga.DataInicio:dd/MM/yyyy}";
                    texto.AppendLine(@$"<tr>
                                        <td>{data}</td>
                                        <td>{sSga.HoraInicio} até {sSga.HoraFim}</td>
                                        <td>{sSga.Local}</td>
                                    </tr>");
                }
                texto.AppendLine(@"</table>
                                    </div>");
            }

            texto.AppendLine(@"</body>
                    </html>");
            return texto.ToString();
        }
    }
}