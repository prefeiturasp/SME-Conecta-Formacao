using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterTipoEncontro : CasoDeUsoAbstrato, ICasoDeUsoObterTipoEncontro
    {
        public CasoDeUsoObterTipoEncontro(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            return await mediator.Send(ObterTipoEncontroQuery.Instancia);
        }
    }
}
