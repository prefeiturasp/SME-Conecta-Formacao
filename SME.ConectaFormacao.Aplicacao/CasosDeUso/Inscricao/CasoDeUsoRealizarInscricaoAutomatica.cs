using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
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
            var propostaInscricaoAutomatica = await mediator.Send(new ObterPropostaInscricaoAutomaticaPorIdQuery(propostaId)) ??
                throw new Exception(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            if (propostaInscricaoAutomatica.Situacao != SituacaoProposta.Publicada || !propostaInscricaoAutomatica.EhInscricaoAutomatica)
                return false;

            if (propostaInscricaoAutomatica.PublicosAlvos.PossuiElementos() && propostaInscricaoAutomatica.PublicosAlvos.Any(t => !t.HasValue))
                throw new Exception(MensagemNegocio.PROPOSTA_COM_PUBLICO_ALVO_SEM_DEPARA_CONFIGURADO.Parametros(propostaId));

            if (propostaInscricaoAutomatica.FuncoesEspecificas.PossuiElementos() && propostaInscricaoAutomatica.FuncoesEspecificas.Any(t => !t.HasValue))
                throw new Exception(MensagemNegocio.PROPOSTA_COM_FUNCAO_ESPECIFICA_SEM_DEPARA_CONFIGURADO.Parametros(propostaId));

            await mediator.Send(new GerarPropostaTurmaVagaCommand(propostaId, propostaInscricaoAutomatica.QuantidadeVagasTurmas));

            var modalidadesEol = ObterModalidadeEol(propostaInscricaoAutomatica.Modalidades);

            var dres = propostaInscricaoAutomatica.PropostasTurmas.Where(t => !string.IsNullOrEmpty(t.CodigoDre)).Select(turma => turma.CodigoDre).Distinct();

            var cursistasEOL = await mediator.Send(new ObterFuncionarioPorFiltroPropostaServicoEolQuery(
                propostaInscricaoAutomatica.PublicosAlvos.Select(t => t.Value),
                propostaInscricaoAutomatica.FuncoesEspecificas.Select(t => t.Value),
                modalidadesEol,
                propostaInscricaoAutomatica.AnosTurmas,
                dres,
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

        private static IEnumerable<long> ObterModalidadeEol(IEnumerable<long> modalidades)
        {
            var retorno = new List<long>();

            foreach (var modalidade in modalidades)
            {
                switch ((Dominio.Enumerados.Modalidade)modalidade)
                {
                    case Dominio.Enumerados.Modalidade.EducacaoInfantil:
                        retorno.Add(1);
                        retorno.Add(10);
                        break;
                    case Dominio.Enumerados.Modalidade.Fundamental:
                        retorno.Add(5);
                        retorno.Add(13);
                        break;
                    case Dominio.Enumerados.Modalidade.Medio:
                        retorno.Add(6);
                        retorno.Add(9);
                        retorno.Add(14);
                        retorno.Add(17);
                        break;
                    default:
                        retorno.Add(modalidade);
                        break;
                }
            }

            return retorno;
        }
    }
}
