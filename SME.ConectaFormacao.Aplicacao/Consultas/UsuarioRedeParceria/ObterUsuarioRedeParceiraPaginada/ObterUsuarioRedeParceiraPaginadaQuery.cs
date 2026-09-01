using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterUsuarioRedeParceiraPaginadaQuery : IRequest<PaginacaoResultadoDto<UsuarioRedeParceriaPaginadoDTO>>
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
