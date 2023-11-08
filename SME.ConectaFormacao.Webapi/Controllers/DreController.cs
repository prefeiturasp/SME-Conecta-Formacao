using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class DreController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterListaDre([FromServices] ICasoDeUsoObterListaDre useCase)
        {
            return Ok(await useCase.Executar());
        }
    }
}