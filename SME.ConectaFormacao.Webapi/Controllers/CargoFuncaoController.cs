using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.CargoFuncao;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class CargoFuncaoController : BaseController
    {
        [HttpGet]
        [HttpGet("tipo/{tipo}")]
        [ProducesResponseType(typeof(IEnumerable<CargoFuncaoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterCargoFuncao(
            [FromServices] ICasoDeUsoObterCargoFuncao casoDeUsoObterCargoFuncao, 
            [FromRoute] CargoFuncaoTipo? tipo,
            [FromQuery] bool exibirOpcaoOutros = false)
        {
            return Ok(await casoDeUsoObterCargoFuncao.Executar(tipo, exibirOpcaoOutros));
        }
    }
}
