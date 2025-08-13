using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricao.ReativarInscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ReativarInscricaoCommandHandler : IRequestHandler<ReativarInscricaoCommand, bool>
    {
        private readonly ITransacao _transacao;
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMediator _mediator;
        private readonly IRepositorioProposta _repositorioProposta;

        public ReativarInscricaoCommandHandler(ITransacao transacao, IRepositorioInscricao repositorioInscricao, IMediator mediator, IRepositorioProposta repositorioProposta)
        {
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(ReativarInscricaoCommand request, CancellationToken cancellationToken)
        {
            var inscricao = await _repositorioInscricao.ObterPorId(request.Id) ??
                throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            if (!inscricao.Situacao.EhCancelada())
                throw new NegocioException(MensagemNegocio.INSCRICAO_SO_PODE_REATIVAR_CANCELADAS);

            var propostaTurma = await _mediator.Send(new ObterPropostaTurmaPorIdQuery(inscricao.PropostaTurmaId), cancellationToken) ??
                               throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

            var proposta = await _mediator.Send(new ObterPropostaPorIdQuery(propostaTurma.PropostaId), cancellationToken) ??
               throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            await ValidarInscricaoAsync(inscricao, proposta);

            SituacaoInscricao situacaoPelaFormacao = ObterSituacaoInscricao(inscricao, proposta, cancellationToken);

            var transacao = _transacao.Iniciar();
            try
            {
                inscricao.Situacao = situacaoPelaFormacao;
                inscricao.MotivoCancelamento = string.Empty;
                await _repositorioInscricao.Atualizar(inscricao);
                transacao.Commit();
                return true;
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }
        }

        public static SituacaoInscricao ObterSituacaoInscricao(Inscricao inscricao, Proposta proposta, CancellationToken cancellationToken)
        {
            if (inscricao.SituacaoAnterior.HasValue && inscricao.SituacaoAnterior.Value != SituacaoInscricao.Cancelada)
            {
                return (SituacaoInscricao)inscricao.SituacaoAnterior;
            }
            return proposta.FormacaoHomologada == FormacaoHomologada.Sim ? SituacaoInscricao.AguardandoAnalise : SituacaoInscricao.Confirmada;
        }

        public async Task ValidarInscricaoAsync(Dominio.Entidades.Inscricao inscricao, Proposta proposta)
        {
            await ValidarCargo(inscricao, proposta.Id);
            await ValidarDre(inscricao);

            if (!proposta.EstaEmPeriodoDeInscricao)
                throw new NegocioException(MensagemNegocio.INSCRICAO_FORA_DO_PERIODO_INSCRICAO);

            // Função desativada temporariamente devido a bug na homologação de limite de vagas. Será reativada após a correção do problema.
            //if (proposta.FormacaoHomologada != FormacaoHomologada.Sim) await ValidarVaga(inscricao, proposta.Id);
        }

        private async Task ValidarCargo(Dominio.Entidades.Inscricao inscricao, long propostaId)
        {
            var publicosAlvo = await _repositorioProposta.ObterPublicoAlvoPorId(propostaId);
            if (!publicosAlvo.Any(p => p.CargoFuncaoId == inscricao.CargoId))
                throw new NegocioException(MensagemNegocio.INSCRICAO_CARGO_NAO_PERMITIDO);
        }

        private async Task ValidarDre(Dominio.Entidades.Inscricao inscricao)
        {
            var turmaDres = await _repositorioProposta.ObterPropostaTurmasDresPorPropostaTurmaId(inscricao.PropostaTurmaId);
            if (!turmaDres.Any(d => d.Dre?.Todos == true || d.DreCodigo == inscricao.CargoDreCodigo))
                throw new NegocioException(MensagemNegocio.INSCRICAO_DRE_NAO_PERMITIDA);
        }

        // Função desativada temporariamente devido a bug na homologação de limite de vagas. Será reativada após a correção do problema.
        private async Task ValidarVaga(Dominio.Entidades.Inscricao inscricao, long propostaId)
        {
            var turmasComVaga = await _repositorioProposta.ObterTurmasComVagaPorId(propostaId, inscricao.CargoDreCodigo);
            if (!turmasComVaga.Any(t => t.Id == inscricao.PropostaTurmaId))
                throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA);
        }
    }
}
