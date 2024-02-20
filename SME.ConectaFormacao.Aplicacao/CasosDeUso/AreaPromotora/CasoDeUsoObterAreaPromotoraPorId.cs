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
            var areaPromotora = await mediator.Send(new ObterAreaPromotoraPorIdQuery(id));
            var grupo = await mediator.Send(new ObterGrupoPorIdQuery(areaPromotora.GrupoId));
            areaPromotora.VisaoId = grupo.VisaoId;
            return areaPromotora;
        }
    }
}
