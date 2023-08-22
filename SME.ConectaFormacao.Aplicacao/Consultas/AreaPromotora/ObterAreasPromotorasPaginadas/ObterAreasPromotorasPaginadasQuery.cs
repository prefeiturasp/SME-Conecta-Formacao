using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreasPromotorasPaginadasQuery : IRequest<PaginacaoResultadoDTO<AreaPromotoraPaginadaDTO>>
    {
        public ObterAreasPromotorasPaginadasQuery(FiltrosAreaPromotoraDTO filtros, int numeroPagina, int numeroRegistros)
        {
            Filtros = filtros;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
        }

        public FiltrosAreaPromotoraDTO Filtros { get; }
        public int NumeroPagina { get; }
        public int NumeroRegistros { get; }
    }
}
