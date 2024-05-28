
using MediatR;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarNotificacaoLidaCommandHandler : IRequestHandler<EnviarNotificacaoLidaCommand, bool>
    {
        private readonly IMediator _mediator;

        public EnviarNotificacaoLidaCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(EnviarNotificacaoLidaCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbitNotificacao.EnviarNotificacaoLidaUsuarios, request.Notificacao, exchange: RotasRabbitNotificacao.Exchange), cancellationToken);
            return true;
        }
    }
}