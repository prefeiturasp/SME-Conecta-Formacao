using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasIdsPorFiltroQuery : IRequest<IEnumerable<long>>
    {
        public ObterPropostasIdsPorFiltroQuery(FiltroListagemFormacaoDTO filtroListagemFormacaoDto)
        {
            FiltroListagemFormacaoDTO = filtroListagemFormacaoDto;
        }

        public FiltroListagemFormacaoDTO FiltroListagemFormacaoDTO { get; }
    }
}
