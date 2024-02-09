using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasIdsPorFiltroQuery : IRequest<IEnumerable<long>>
    {
        public ObterPropostasIdsPorFiltroQuery(FiltroListagemFormacaoDTO filtroListagemFormacaoDto, int numeroPagina, int numeroRegistros)
        {
            FiltroListagemFormacaoDTO = filtroListagemFormacaoDto;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
        }

        public int NumeroPagina { get; set; }
        public int NumeroRegistros { get; set; }

        public FiltroListagemFormacaoDTO FiltroListagemFormacaoDTO { get; }
    }
}
