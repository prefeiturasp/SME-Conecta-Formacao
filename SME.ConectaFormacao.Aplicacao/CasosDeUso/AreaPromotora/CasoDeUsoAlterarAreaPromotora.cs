using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora
{
    public class CasoDeUsoAlterarAreaPromotora : CasoDeUsoAbstrato, ICasoDeUsoAlterarAreaPromotora
    {
        public CasoDeUsoAlterarAreaPromotora(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long id, AreaPromotoraDTO areaPromotoraDTO)
        {
            return await mediator.Send(new AlterarAreaPromotoraCommand(id, areaPromotoraDTO));
        }
    }
}
