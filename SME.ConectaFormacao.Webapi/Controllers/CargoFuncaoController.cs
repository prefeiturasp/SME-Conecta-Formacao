using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Route("api/v1/cargo-funcao")]
    public class CargoFuncaoController : BaseController
    {
        [HttpGet]
        [HttpGet("tipo/{tipo}")]
        [ProducesResponseType(typeof(IEnumerable<CargoFuncaoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> ObterCargoFuncao([FromServices] ICasoDeUsoObterCargoFuncao casoDeUsoObterCargoFuncao, [FromRoute] CargoFuncaoTipo? tipo)
        {
            return Ok(await casoDeUsoObterCargoFuncao.Executar(tipo));
        }
    }
}
