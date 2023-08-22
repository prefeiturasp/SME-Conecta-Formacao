using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora
{
    public class CasoDeUsoInserirAreaPromotora : CasoDeUsoAbstrato, ICasoDeUsoInserirAreaPromotora
    {
        public CasoDeUsoInserirAreaPromotora(IMediator mediator) : base(mediator)
        {
        }

        public async Task<long> Executar(InserirAreaPromotoraDTO inserirAreaPromotoraDTO)
        {
            return await mediator.Send(new InserirAreaPromotoraCommand(inserirAreaPromotoraDTO));
        }
    }
}
