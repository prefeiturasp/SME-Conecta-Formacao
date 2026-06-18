using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementar;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class CodafSuplementarController : BaseController
    {
        [HttpGet("codaf/{codafId:long}")]
        public async Task<IActionResult> ObterPorCodafIdAsync(long codafId, [FromServices] ICasoDeUsoObterCodafSuplementarPorCodafId casoDeUso)
        {
            var resultado = await casoDeUso.ExecutarAsync(codafId);
            return ProcessarResultado(resultado);
        }
    }
}