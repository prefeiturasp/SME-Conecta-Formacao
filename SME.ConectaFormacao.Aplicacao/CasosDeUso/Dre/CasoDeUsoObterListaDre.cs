using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Aplicacao.Dtos;

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