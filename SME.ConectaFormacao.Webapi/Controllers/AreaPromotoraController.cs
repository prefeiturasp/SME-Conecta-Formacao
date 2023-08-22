using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class AreaPromotoraController : BaseController
    {
        [HttpGet("tipos")]
        [ProducesResponseType(typeof(IEnumerable<AreaPromotoraTipoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> ObterGrupos([FromServices] ICasoDeUsoObterTiposAreaPromotora casoDeUsoObterTiposAreaPromotora)
        {
            return Ok(await casoDeUsoObterTiposAreaPromotora.Executar());
        }
    }
}
