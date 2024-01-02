using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaMovimentacaoCommandHandler : IRequestHandler<SalvarPropostaMovimentacaoCommand, bool>
    {
        private readonly IRepositorioPropostaMovimentacao _repositorioPropostaMovimentacao;
        private readonly IMediator _mediator;

        public SalvarPropostaMovimentacaoCommandHandler(IRepositorioPropostaMovimentacao repositorioPropostaMovimentacao, IMediator mediator)
        {
            _repositorioPropostaMovimentacao = repositorioPropostaMovimentacao ?? throw new ArgumentNullException(nameof(repositorioPropostaMovimentacao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(SalvarPropostaMovimentacaoCommand request, CancellationToken cancellationToken)
        {
            var propostaMovimentacao = new PropostaMovimentacao
            {
                PropostaId = request.PropostaId,
                Situacao = request.Situacao,
                Justificativa = request.Justificativa
            };

            await _repositorioPropostaMovimentacao.Inserir(propostaMovimentacao);

            await RemoverCaches(request.PropostaId, cancellationToken);

            return true;
        }

        private async Task RemoverCaches(long propostaId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.FormacaoResumida.Parametros(propostaId)), cancellationToken);
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.FormacaoDetalhada.Parametros(propostaId)), cancellationToken);

            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.Proposta.Parametros(propostaId)), cancellationToken);
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.PropostaPublicoAlvo.Parametros(propostaId)), cancellationToken);
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.PropostaFuncaoEspecifica.Parametros(propostaId)), cancellationToken);
        }
    }
}