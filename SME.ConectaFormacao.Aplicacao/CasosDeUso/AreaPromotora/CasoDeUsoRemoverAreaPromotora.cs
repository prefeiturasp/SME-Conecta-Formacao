using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora
{
    public class CasoDeUsoRemoverAreaPromotora : CasoDeUsoAbstrato, ICasoDeUsoRemoverAreaPromotora
    {
        public CasoDeUsoRemoverAreaPromotora(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long id)
        {
            return await mediator.Send(new RemoverAreaPromotoraCommand(id));
        }
    }
}
