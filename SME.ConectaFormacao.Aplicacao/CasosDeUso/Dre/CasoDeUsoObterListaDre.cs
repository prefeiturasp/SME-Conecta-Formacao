using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CasoDeUsoObterListaDre : CasoDeUsoAbstrato, ICasoDeUsoObterListaDre
    {
        public CasoDeUsoObterListaDre(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar(bool exibirTodos)
        {
            return await mediator.Send(new ObterListaDreQuery(exibirTodos));
        }
    }
}