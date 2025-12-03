using MediatR;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso
{
    public abstract class CasoDeUsoAbstrato(IMediator mediator)
    {
        protected readonly IMediator mediator = mediator;
    }
}
