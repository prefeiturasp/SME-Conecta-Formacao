using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
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

            if(propostaInscricaoAutomatica.Situacao != SituacaoProposta.Publicada || !propostaInscricaoAutomatica.EhInscricaoAutomatica)
                return false;

            var cursistasEOL = await mediator.Send(new ObterFuncionarioPorFiltroPropostaServicoEolQuery(
                propostaInscricaoAutomatica.PublicosAlvos,
                propostaInscricaoAutomatica.FuncoesEspecificas,
                propostaInscricaoAutomatica.Modalidades,
                propostaInscricaoAutomatica.AnosTurmas,
                propostaInscricaoAutomatica.PropostasTurmas.Select(turma => turma.CodigoDre).Distinct(),
                propostaInscricaoAutomatica.ComponentesCurriculares,
                propostaInscricaoAutomatica.EhTipoJornadaJEIF));

            var quantidadeMaximaCursistaPorTurma = int.MaxValue;

            if (propostaInscricaoAutomatica.IntegrarNoSGA)
            {
                ParametroSistema qtdeCursistasSuportadosPorTurma = await ObterParametroQtdeCursistasSuportadosPorTurma();
                quantidadeMaximaCursistaPorTurma = int.Parse(qtdeCursistasSuportadosPorTurma.Valor);
            }

            var inscricaoAutomaticaTratarTurmas = new InscricaoAutomaticaTratarTurmasDTO
            {
                PropostaInscricaoAutomatica = propostaInscricaoAutomatica,
                CursistasEOL = cursistasEOL,
                QtdeCursistasSuportadosPorTurma = quantidadeMaximaCursistaPorTurma
            };

            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.RealizarInscricaoAutomaticaTratarTurmas, inscricaoAutomaticaTratarTurmas));

            return true;
        }

        private async Task<ParametroSistema> ObterParametroQtdeCursistasSuportadosPorTurma()
        {
            var anoAtual = DateTimeExtension.HorarioBrasilia().Year;
            var qtdeCursistasSuportadosPorTurma = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.QtdeCursistasSuportadosPorTurma, anoAtual));

            if (qtdeCursistasSuportadosPorTurma.Valor.NaoEstaPreenchido())
                throw new NegocioException(string.Format(MensagemNegocio.PARAMETRO_QTDE_CURSISTAS_SUPORTADOS_POR_TURMA_NAO_ENCONTRADO, anoAtual));
            return qtdeCursistasSuportadosPorTurma;
        }
    }
}
