using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioRedeParceiraPaginadaQuery : IRequest<PaginacaoResultadoDTO<UsuarioRedeParceriaPaginadoDTO>>
    {
        public ObterUsuarioRedeParceiraPaginadaQuery(FiltroUsuarioRedeParceriaDTO filtros, int numeroPagina, int numeroRegistros)
        {
            Filtros = filtros;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
        }

        public FiltroUsuarioRedeParceriaDTO Filtros { get; }
        public int NumeroPagina { get; }
        public int NumeroRegistros { get; }
    }
}
