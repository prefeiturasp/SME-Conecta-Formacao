using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriasSelect
{
    public class ObterCoordenadoriasSelectQuery() : IRequest<List<CoordenadoriaDto>>
    {
        public string? Sigla { get; set; } 
        public string? Nome { get; set; } 
    }
}