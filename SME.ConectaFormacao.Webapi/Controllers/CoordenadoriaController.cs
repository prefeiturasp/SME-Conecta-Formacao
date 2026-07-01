using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.AlterarCoordenadoria;
using SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.InserirCoordenadoria;
using SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.RemoverCoordenadoria;
using SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriaPorId;
using SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriasPaginado;
using SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriasSelect;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class CoordenadoriaController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(Resultado<CoordenadoriaDto>), 201)]
        [ProducesResponseType(typeof(Resultado<CoordenadoriaDto>), 400)]
        public async Task<IActionResult> Cadastrar(
            [FromBody] CoordenadoriaCadastroDto coordenadoriaCadastroDto,
            [FromServices] IMediator mediator)
        {
            var resultado = await mediator.Send(new InserirCoordenadoriaCommand(coordenadoriaCadastroDto.Nome, coordenadoriaCadastroDto.Sigla));
            return ProcessarCriado(resultado);
        }

        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(Resultado), 200)]
        [ProducesResponseType(typeof(Resultado), 400)]
        public async Task<IActionResult> Alterar(
            long id,
            [FromBody] CoordenadoriaCadastroDto coordenadoriaCadastroDto,
            [FromServices] IMediator mediator)
        {
            var resultado = await mediator.Send(new AlterarCoordenadoriaCommand(id, coordenadoriaCadastroDto.Nome, coordenadoriaCadastroDto.Sigla));
            return ProcessarResultado(resultado);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CoordenadoriaDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CoordenadoriaDto>>), 400)]
        public async Task<IActionResult> ObterCoordenadoriasPaginado(
            [FromQuery] CoordenadoriaFiltroDto filtro,
            [FromServices] IMediator mediator,
            [FromServices] IContextoAplicacao contextoAplicacao)
        {
            if (!int.TryParse(contextoAplicacao.ObterVariavel<string>("NumeroPagina"), out var numeroPagina) || numeroPagina == 0) numeroPagina = filtro.NumeroPagina;
            if (!int.TryParse(contextoAplicacao.ObterVariavel<string>("NumeroRegistros"), out var numeroRegistros) || numeroRegistros == 0) numeroRegistros = filtro.NumeroRegistros;
            var resultado = await mediator.Send(new ObterCoordenadoriasPaginadoQuery(filtro.Nome, filtro.Sigla, numeroPagina, numeroRegistros));
            return ProcessarResultado(resultado);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(Resultado<CoordenadoriaDto>), 200)]
        [ProducesResponseType(typeof(Resultado<CoordenadoriaDto>), 404)]
        public async Task<IActionResult> ObterPorId(
            long id,
            [FromServices] IMediator mediator)
        {
            var resultado = await mediator.Send(new ObterCoordenadoriaPorIdQuery(id));
            return ProcessarResultado(resultado);
        }

        [HttpDelete("{id:long}")]
        [ProducesResponseType(typeof(Resultado), 204)]
        [ProducesResponseType(typeof(Resultado), 400)]
        public async Task<IActionResult> Excluir(
            long id,
            [FromServices] IMediator mediator)
        {
            var resultado = await mediator.Send(new RemoverCoordenadoriaCommand(id));
            return ProcessarResultado(resultado);
        }

        [HttpGet("select")]
        [ProducesResponseType(typeof(List<CoordenadoriaDto>), 200)]
        [ProducesResponseType(typeof(Resultado), 400)]
        public async Task<List<CoordenadoriaDto>> ObterSelectCoordenadorias([FromServices] IMediator mediator)
        {
          return await mediator.Send(new ObterCoordenadoriasSelectQuery());
        }
    }
}