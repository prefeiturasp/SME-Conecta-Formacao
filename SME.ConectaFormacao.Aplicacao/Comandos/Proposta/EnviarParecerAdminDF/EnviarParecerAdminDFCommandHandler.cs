using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarParecerAdminDFCommandHandler : IRequestHandler<EnviarParecerAdminDFCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMediator _mediator;

        public EnviarParecerAdminDFCommandHandler(IRepositorioProposta repositorioProposta, IMediator mediator)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(EnviarParecerAdminDFCommand request, CancellationToken cancellationToken)
        {
            if (await _repositorioProposta.SituacaoPropostaEhAguardandoAnaliseParecerDf(request.IdProposta))
            {
                await _mediator.Send(new EnviarPropostaCommand(request.IdProposta, SituacaoProposta.AnaliseParecerAreaPromotora));
                await _mediator.Send(new SalvarPropostaMovimentacaoCommand(request.IdProposta, SituacaoProposta.AnaliseParecerAreaPromotora));

                await _repositorioProposta.AtualizarSituacaoDoParecerEnviadaPeloAdminDF(request.IdProposta);

                return true;
            }

            return false;
        }
    }
}
