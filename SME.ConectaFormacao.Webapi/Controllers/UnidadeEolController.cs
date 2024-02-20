using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ue;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class UnidadeEolController : BaseController
    {
        [HttpGet("{codigoEol}")]
        [ProducesResponseType(typeof(UnidadeEol), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> BuscarUnidadePorCodigoEol(
            [FromRoute] string codigoEol,
            [FromServices] ICasoDeUsoObterUnidadePorCodigoEol useCase
            )
        {
            return Ok(await useCase.Executar(codigoEol));
        }
    }
}