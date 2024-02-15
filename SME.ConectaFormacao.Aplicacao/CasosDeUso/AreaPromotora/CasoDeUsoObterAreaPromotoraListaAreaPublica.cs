using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora
{
    public class CasoDeUsoObterAreaPromotoraListaAreaPublica : CasoDeUsoAbstrato, ICasoDeUsoObterAreaPromotoraListaAreaPublica
    {
        public CasoDeUsoObterAreaPromotoraListaAreaPublica(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            return await mediator.Send(new ObterAreaPromotoraListaQuery(null));
        }
    }
}
