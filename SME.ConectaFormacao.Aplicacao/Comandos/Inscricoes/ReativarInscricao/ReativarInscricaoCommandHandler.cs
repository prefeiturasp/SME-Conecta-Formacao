using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.ReativarInscricao
{
    public class ReativarInscricaoCommandHandler(ITransacao transacao, IRepositorioInscricao repositorioInscricao, IMediator mediator, IRepositorioProposta repositorioProposta) : IRequestHandler<ReativarInscricaoCommand, bool>
    {
        private readonly ITransacao _transacao = transacao;

        public async Task<bool> Handle(ReativarInscricaoCommand request, CancellationToken cancellationToken)
        {
            var inscricao = await repositorioInscricao.ObterPorId(request.Id) ??
                throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            if (inscricao.Situacao != SituacaoInscricao.Cancelada)
                throw new NegocioException(MensagemNegocio.INSCRICAO_SO_PODE_REATIVAR_CANCELADAS);

            var propostaTurma = await mediator.Send(new ObterPropostaTurmaPorIdQuery(inscricao.PropostaTurmaId), cancellationToken) ??
                               throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaTurma.PropostaId), cancellationToken) ??
               throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            await ValidarInscricaoAsync(inscricao, proposta);

            SituacaoInscricao situacaoPelaFormacao = ObterSituacaoInscricao(inscricao, proposta, cancellationToken);

            var transacao = _transacao.Iniciar();
            try
            {
                inscricao.Situacao = situacaoPelaFormacao;
                inscricao.MotivoCancelamento = string.Empty;
                await repositorioInscricao.Atualizar(inscricao);
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

        public async Task ValidarInscricaoAsync(Inscricao inscricao, Proposta proposta)
        {
            await ValidarCargo(inscricao, proposta.Id);
            await ValidarDre(inscricao);

            if (!proposta.EstaEmPeriodoDeInscricao)
                throw new NegocioException(MensagemNegocio.INSCRICAO_FORA_DO_PERIODO_INSCRICAO);
        }

        private async Task ValidarCargo(Inscricao inscricao, long propostaId)
        {
            var publicosAlvo = await repositorioProposta.ObterPublicoAlvoPorId(propostaId);
            if (!publicosAlvo.Any(p => p.CargoFuncaoId == inscricao.CargoId))
                throw new NegocioException(MensagemNegocio.INSCRICAO_CARGO_NAO_PERMITIDO);
        }

        private async Task ValidarDre(Inscricao inscricao)
        {
            var turmaDres = await repositorioProposta.ObterPropostaTurmasDresPorPropostaTurmaId(inscricao.PropostaTurmaId);
            if (!turmaDres.Any(d => d.Dre?.Todos == true || d.DreCodigo == inscricao.CargoDreCodigo))
                throw new NegocioException(MensagemNegocio.INSCRICAO_DRE_NAO_PERMITIDA);
        }
    }
}
