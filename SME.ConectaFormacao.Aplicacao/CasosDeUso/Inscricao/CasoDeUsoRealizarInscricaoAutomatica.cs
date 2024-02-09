using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoRealizarInscricaoAutomatica : CasoDeUsoAbstrato, ICasoDeUsoRealizarInscricaoAutomatica
    {
        public CasoDeUsoRealizarInscricaoAutomatica(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var propostaId = Convert.ToInt64(param.Mensagem);
            var propostaInscricaoAutomatica = await mediator.Send(new ObterPropostaInscricaoAutomaticaPorIdQuery(propostaId));

            if (propostaInscricaoAutomatica.Situacao != SituacaoProposta.Publicada || !propostaInscricaoAutomatica.EhInscricaoAutomatica)
                return false;

            await mediator.Send(new GerarPropostaTurmaVagaCommand(propostaId, propostaInscricaoAutomatica.QuantidadeVagasTurmas));

            var cursistasEOL = await mediator.Send(new ObterFuncionarioPorFiltroPropostaServicoEolQuery(
                propostaInscricaoAutomatica.PublicosAlvos,
                propostaInscricaoAutomatica.FuncoesEspecificas,
                propostaInscricaoAutomatica.Modalidades,
                propostaInscricaoAutomatica.AnosTurmas,
                propostaInscricaoAutomatica.PropostasTurmas.Select(turma => turma.CodigoDre).Distinct(),
                propostaInscricaoAutomatica.ComponentesCurriculares,
                propostaInscricaoAutomatica.EhTipoJornadaJEIF));

            var inscricaoAutomaticaTratarTurmas = new InscricaoAutomaticaTratarTurmasDTO
            {
                PropostaInscricaoAutomatica = propostaInscricaoAutomatica,
                CursistasEOL = cursistasEOL
            };

            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.RealizarInscricaoAutomaticaTratarTurmas, inscricaoAutomaticaTratarTurmas));

            return true;
        }
    }
}
