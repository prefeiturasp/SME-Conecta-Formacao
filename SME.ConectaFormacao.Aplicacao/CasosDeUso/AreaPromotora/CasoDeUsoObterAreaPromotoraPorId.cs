using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora
{
    public class CasoDeUsoObterAreaPromotoraPorId : CasoDeUsoAbstrato, ICasoDeUsoObterAreaPromotoraPorId
    {
        public CasoDeUsoObterAreaPromotoraPorId(IMediator mediator) : base(mediator)
        {
        }

        public async Task<AreaPromotoraCompletoDTO> Executar(long id)
        {
            return await mediator.Send(new ObterAreaPromotoraPorIdQuery(id));
        }
    }
}
