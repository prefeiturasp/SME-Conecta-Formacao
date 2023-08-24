using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Grupo;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class GrupoController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GrupoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> ObterGrupos([FromServices] ICasoDeUsoObterGrupos casoDeUsoObterGrupos)
        {
            return Ok(await casoDeUsoObterGrupos.Executar());
        }
    }
}
