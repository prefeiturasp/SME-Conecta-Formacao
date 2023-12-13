using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasIdsPorFiltroQuery : IRequest<IEnumerable<long>>
    {
        public ObterPropostasIdsPorFiltroQuery(FiltroListagemFormacaoDTO filtroListagemFormacaoDto, int numeroLinha, int numeroRegistros)
        {
            FiltroListagemFormacaoDTO = filtroListagemFormacaoDto;
            NumeroLinha = numeroLinha;
            NumeroRegistros = numeroRegistros;
        }

        public int NumeroRegistros { get; set; }
        public int NumeroLinha { get; set; }
        public FiltroListagemFormacaoDTO FiltroListagemFormacaoDTO { get; }
    }
}
