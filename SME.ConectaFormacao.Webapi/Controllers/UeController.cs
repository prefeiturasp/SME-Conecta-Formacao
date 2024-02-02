using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ue;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class UeController : BaseController
    {
        [HttpGet("{ueCodigo}")]
        [ProducesResponseType(typeof(UeServicoEol), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> BuscarUePorCodigo(
            [FromRoute] string ueCodigo,
            [FromServices] ICasoDeUsoObterUePorCodigo useCase
            )
        {
            return Ok(await useCase.Executar(ueCodigo));
        }
    }
}