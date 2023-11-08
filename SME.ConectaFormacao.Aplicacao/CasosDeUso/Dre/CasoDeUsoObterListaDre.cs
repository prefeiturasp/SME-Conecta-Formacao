using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Dre;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CasoDeUsoObterListaDre : CasoDeUsoAbstrato,ICasoDeUsoObterListaDre
    {
        public CasoDeUsoObterListaDre(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            return await mediator.Send(ObterListaDreQuery.Instancia);
        }
    }
}