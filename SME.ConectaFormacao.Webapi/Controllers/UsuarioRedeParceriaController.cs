using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class UsuarioRedeParceriaController : BaseController
    {
        [HttpGet("situacao")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> ObterSituacao(
            [FromServices] ICasoDeUsoObterSituacaoUsuarioRedeParceria useCase)
        {
            return Ok(await useCase.Executar());
        }
    }
}
