using MediatR;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso
{
    public abstract class CasoDeUsoAbstrato
    {
        protected readonly IMediator mediator;

        protected CasoDeUsoAbstrato(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
    }
}
