using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriasPaginado
{
    public class ObterCoordenadoriasPaginadoQuery(string? Nome, string? Sigla, int Pagina = 1, int TamanhoPagina = 1) : IRequest<Resultado<PaginacaoResultadoDto<CoordenadoriaDto>>>
    {
        public string? Nome { get; set; } = Nome;
        public string? Sigla { get; set; } = Sigla;
        public int Pagina { get; set; } = Pagina;
        public int TamanhoPagina { get; set; } = TamanhoPagina;
    }
}