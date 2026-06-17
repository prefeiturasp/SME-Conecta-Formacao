using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriasSelect
{
    public class ObterCoordenadoriasSelectQueryHandler(IRepositorioCoordenadoria repositorioCoordenadoria) : IRequestHandler<ObterCoordenadoriasSelectQuery, List<CoordenadoriaDto>>
    {
        public async Task<List<CoordenadoriaDto>> Handle(ObterCoordenadoriasSelectQuery request, CancellationToken cancellationToken)
        {
            var resultado = await repositorioCoordenadoria
                .ObterCoordenadoriaSelectAsync();
            var resultadoDto = new List<CoordenadoriaDto>(
                resultado.Select(c => new CoordenadoriaDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Sigla = c.Sigla,
                    NomeComSigla = string.IsNullOrEmpty(c.Sigla) ? c.Nome : $"{c.Sigla} - {c.Nome}"
                })
            );  
            return resultadoDto;
        }
    }
}
