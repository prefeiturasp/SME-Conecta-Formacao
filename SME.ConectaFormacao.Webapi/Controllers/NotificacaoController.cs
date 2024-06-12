using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class NotificacaoController : BaseController
    {
        [HttpGet("categoria")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterCategoriaNotificacao(
            [FromServices] ICasoDeUsoObterCategoriaNotificacao casoDeUso)
        {
            return Ok(await casoDeUso.Executar());
        }

        [HttpGet("tipo")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterTipoNotificacao(
            [FromServices] ICasoDeUsoObterTipoNotificacao casoDeUso)
        {
            return Ok(await casoDeUso.Executar());
        }

        [HttpGet("situacao")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterSituacaoNotificacao(
            [FromServices] ICasoDeUsoObterSituacaoNotificacao casoDeUso)
        {
            return Ok(await casoDeUso.Executar());
        }

        [HttpGet("nao-lida")]
        [ProducesResponseType(typeof(long), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterTotalNotificacaoNaoLida(
            [FromServices] ICasoDeUsoObterTotalNotificacaoNaoLida casoDeUso)
        {
            return Ok(await casoDeUso.Executar());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NotificacaoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterNotificacao(
            [FromServices] ICasoDeUsoObterNotificacao casoDeUso,
            [FromRoute] long id)
        {
            return Ok(await casoDeUso.Executar(id));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginacaoResultadoDTO<NotificacaoPaginadoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterNotificacoes(
            [FromServices] ICasoDeUsoObterNotificacaoPaginada casoDeUso,
            [FromQuery] NotificacaoFiltroDTO filtro)
        {
            return Ok(await casoDeUso.Executar(filtro));
        }
    }
}
