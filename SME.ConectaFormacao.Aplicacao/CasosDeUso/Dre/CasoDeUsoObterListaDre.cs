using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Dre;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CasoDeUsoObterListaDre : CasoDeUsoAbstrato, ICasoDeUsoObterListaDre
    {
        public CasoDeUsoObterListaDre(IMediator mediator) : base(mediator)
        {
        }

        public Task<IEnumerable<DreDTO>> Executar(bool exibirTodos)
        {
            return mediator.Send(new ObterListaDreQuery(exibirTodos));
        }
    }
}