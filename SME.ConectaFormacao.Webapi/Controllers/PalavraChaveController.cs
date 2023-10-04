using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Dtos.PalavraChave;
using SME.ConectaFormacao.Aplicacao.Interfaces.PalavraChave;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class PalavraChaveController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PalavraChaveDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterPalavraChave([FromServices] ICasoDeUsoObterPalavraChave casoDeUsoObterPalavraChave)
        {
            return Ok(await casoDeUsoObterPalavraChave.Executar());
        }
    }
}
