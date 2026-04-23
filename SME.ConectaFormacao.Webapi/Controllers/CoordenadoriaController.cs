using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.AlterarCoordenadoria;
using SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.InserirCoordenadoria;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;
using SME.ConectaFormacao.Dominio.Comum;

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
    }
}
