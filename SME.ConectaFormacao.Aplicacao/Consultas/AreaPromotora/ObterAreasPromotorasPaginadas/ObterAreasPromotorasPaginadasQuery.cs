using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreasPromotorasPaginadasQuery : IRequest<PaginacaoResultadoDTO<AreaPromotoraPaginadaDTO>>
    {
        public ObterAreasPromotorasPaginadasQuery(AreaPromotoraFiltrosDTO filtros, int numeroPagina, int numeroRegistros)
        {
            Filtros = filtros;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
        }

        public AreaPromotoraFiltrosDTO Filtros { get; }
        public int NumeroPagina { get; }
        public int NumeroRegistros { get; }
    }
}
