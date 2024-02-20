using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CancelarInscricaoCommandHandler : IRequestHandler<CancelarInscricaoCommand, bool>
    {
        private readonly ITransacao _transacao;
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMediator _mediator;

        public CancelarInscricaoCommandHandler(ITransacao transacao, IRepositorioInscricao repositorioInscricao, IMediator mediator)
        {
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(CancelarInscricaoCommand request, CancellationToken cancellationToken)
        {
            var inscricao = await _repositorioInscricao.ObterPorId(request.Id) ??
                throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            var propostaTurma = await _mediator.Send(new ObterPropostaTurmaPorIdQuery(inscricao.PropostaTurmaId), cancellationToken) ??
                throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

            var proposta = await _mediator.Send(new ObterPropostaPorIdQuery(propostaTurma.PropostaId), cancellationToken) ??
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            var formacaoHomologada = proposta.FormacaoHomologada == FormacaoHomologada.Sim;

            var transacao = _transacao.Iniciar();
            try
            {
                inscricao.Situacao = SituacaoInscricao.Cancelada;
                await _repositorioInscricao.Atualizar(inscricao);

                if (!formacaoHomologada)
                    await _repositorioInscricao.LiberarInscricaoVaga(inscricao);

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
    }
}
