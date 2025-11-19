using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Dominio.Interfaces;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarEmailConfirmacaoInscricaoCommandHandler(
        IRepositorioInscricao repositorioInscricao,
        IMediator mediator,
        IServicoTemplateEmail servicoTemplateEmail) : IRequestHandler<EnviarEmailConfirmacaoInscricaoCommand, bool>
    {
        public async Task<bool> Handle(EnviarEmailConfirmacaoInscricaoCommand request, CancellationToken cancellationToken)
        {
            var dadosParaEmail = await repositorioInscricao.ObterDadosInscricaoPorInscricaoId(request.InscricaoId);
            if (dadosParaEmail is null || !dadosParaEmail.Any())
                return false;

            var dadosDestinatario = dadosParaEmail.FirstOrDefault()!;
            var destinatario = new EnviarEmailDto
            {
                EmailDestinatario = dadosDestinatario.Email,
                NomeDestinatario = dadosDestinatario.NomeDestinatario,
                Titulo = $"Inscrição confirmada | {dadosDestinatario.NomeFormacao} ",
                Texto = await servicoTemplateEmail.ObterHtmlInscricaoConfirmadaAsync(dadosDestinatario.NomeDestinatario, dadosDestinatario.NomeFormacao)
            };
            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, destinatario), cancellationToken);

            return true;
        }
    }
}