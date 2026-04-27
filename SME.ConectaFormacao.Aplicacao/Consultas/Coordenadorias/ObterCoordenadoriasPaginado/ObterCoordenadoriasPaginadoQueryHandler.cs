using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriasPaginado
{
    public class ObterCoordenadoriasPaginadoQueryHandler(IRepositorioCoordenadoria repositorioCoordenadoria) : IRequestHandler<ObterCoordenadoriasPaginadoQuery, Resultado<PaginacaoResultadoDto<CoordenadoriaDto>>>
    {
        public async Task<Resultado<PaginacaoResultadoDto<CoordenadoriaDto>>> Handle(ObterCoordenadoriasPaginadoQuery request, CancellationToken cancellationToken)
        {
            var resultado = await repositorioCoordenadoria
                .ObterCoordenadoriaPaginadoAsync(request.Nome, request.Sigla, request.Pagina, request.TamanhoPagina);
            var resultadoDto = new PaginacaoResultadoDto<CoordenadoriaDto>(
                items: [.. resultado.Itens.Select(c => new CoordenadoriaDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Sigla = c.Sigla
                })],
                numeroRegistros: resultado.TamanhoPagina,
                totalRegistros: resultado.TotalRegistros
            );
            return resultadoDto;
        }
    }
}
