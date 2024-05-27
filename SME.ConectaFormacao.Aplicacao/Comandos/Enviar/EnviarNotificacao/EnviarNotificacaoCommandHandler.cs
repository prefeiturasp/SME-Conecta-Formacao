
using MediatR;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarNotificacaoCommandHandler : IRequestHandler<EnviarNotificacaoCommand, bool>
    {
        private readonly IMediator _mediator;

        public EnviarNotificacaoCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(EnviarNotificacaoCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarNotificacaoCriadaUsuarios, request.Notificacao));

            return true;
        }
    }
}