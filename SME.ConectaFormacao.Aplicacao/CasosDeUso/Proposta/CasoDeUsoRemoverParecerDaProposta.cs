using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoRemoverParecerDaProposta : CasoDeUsoAbstrato ,ICasoDeUsoRemoverParecerDaProposta
    {
        public CasoDeUsoRemoverParecerDaProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long parecerId)
        {
            return await mediator.Send(new ExcluirParecerCommand(parecerId));
        }
    }
}