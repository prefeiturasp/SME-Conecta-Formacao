using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class NotificacaoController : BaseController
    {
        [HttpGet("nao-lida")]
        [ProducesResponseType(typeof(long), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> ObterTotalNotificacaoNaoLida(
            [FromServices] ICasoDeUsoObterTotalNotificacaoNaoLida casoDeUso)
        {
            return Ok(await casoDeUso.Executar());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NotificacaoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> ObterNotificacao(
            [FromServices] ICasoDeUsoObterTotalNotificacaoNaoLida casoDeUso)
        {
            return Ok(await casoDeUso.Executar());
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginacaoResultadoDTO<NotificacaoPaginadoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> ObterNotificacoes(
            [FromServices] ICasoDeUsoObterNotificacaoPaginada casoDeUso,
            [FromQuery] NotificacaoFiltroDTO filtro)
        {
            return Ok(await casoDeUso.Executar(filtro));
        }
    }
}
