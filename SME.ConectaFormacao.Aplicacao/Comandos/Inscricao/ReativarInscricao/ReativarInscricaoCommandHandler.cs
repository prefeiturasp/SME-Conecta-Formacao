using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricao.ReativarInscricao
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

            await ValidarInscricaoAsync(inscricao);

            var transacao = _transacao.Iniciar();
            try
            {
                inscricao.Situacao = SituacaoInscricao.anter;
                inscricao.MotivoCancelamento = null;
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

        public async Task ValidarInscricaoAsync(Dominio.Entidades.Inscricao inscricao)
        {
            await ValidarCargo(inscricao);
            await ValidarDre(inscricao);
            await ValidarVaga(inscricao);
        }

        private async Task ValidarCargo(Dominio.Entidades.Inscricao inscricao)
        {
            // 1. Validar se tem o mesmo cargo (público-alvo)
            var publicosAlvo = await _repositorioProposta.ObterPublicoAlvoPorId(inscricao.PropostaTurmaId);
            if (!publicosAlvo.Any(p => p.CargoFuncaoId.ToString() == inscricao.CargoCodigo))
                throw new NegocioException(MensagemNegocio.INSCRICAO_CARGO_NAO_PERMITIDO);
        }

        private async Task ValidarDre(Dominio.Entidades.Inscricao inscricao)
        {
            // 2. Validar se tem a mesma DRE cadastrada nas propostas
            var turmaDres = await _repositorioProposta.ObterPropostaTurmasDresPorPropostaTurmaId(inscricao.PropostaTurmaId);
            if (!turmaDres.Any(d => d.DreCodigo == inscricao.CargoDreCodigo))
                throw new NegocioException(MensagemNegocio.INSCRICAO_DRE_NAO_PERMITIDA);
        }

        private async Task ValidarVaga(Dominio.Entidades.Inscricao inscricao)
        {
            // 3. Validar se há vaga 
            var turmasComVaga = await _repositorioProposta.ObterTurmasComVagaPorId(inscricao.PropostaTurmaId, inscricao.CargoDreCodigo);
            if (!turmasComVaga.Any(t => t.Id == inscricao.PropostaTurmaId))
                throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA);
        }
    }
}
