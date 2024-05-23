using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class NotificacaoController : BaseController
    {
        [HttpGet("nao-lida")]
        [ProducesResponseType(typeof(long), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> ObterModalidade(
            [FromServices] ICasoDeUsoObterTotalNotificacaoNaoLida casoDeUso)
        {
            return Ok(await casoDeUso.Executar());
        }
    }
}
