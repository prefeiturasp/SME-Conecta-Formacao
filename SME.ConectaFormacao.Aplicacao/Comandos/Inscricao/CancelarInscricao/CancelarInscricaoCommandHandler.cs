using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
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

            var transacao = _transacao.Iniciar();
            try
            {
                if (inscricao.Situacao.EhConfirmada())
                    await _repositorioInscricao.LiberarInscricaoVaga(inscricao);

                inscricao.Situacao = SituacaoInscricao.Cancelada;

                if (request.Motivo.EstaPreenchido())
                    inscricao.MotivoCancelamento = request.Motivo;

                await _repositorioInscricao.Atualizar(inscricao);
                await _mediator.Send(new EnviarEmailCancelarInscricaoCommand(request.Id, request.Motivo), cancellationToken);
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
