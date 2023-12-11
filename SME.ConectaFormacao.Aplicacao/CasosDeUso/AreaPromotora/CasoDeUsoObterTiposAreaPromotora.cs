using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora
{
    public class CasoDeUsoObterTiposAreaPromotora : CasoDeUsoAbstrato, ICasoDeUsoObterTiposAreaPromotora
    {
        public CasoDeUsoObterTiposAreaPromotora(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<AreaPromotoraTipoDTO>> Executar()
        {
            return await mediator.Send(new ObterTiposAreaPromotoraQuery());
        }
    }
}
