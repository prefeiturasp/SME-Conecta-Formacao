using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class InscricaoController : BaseController
    {
        [HttpGet("dados-inscricao")]
        [ProducesResponseType(typeof(DadosUsuarioInscricaoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> ObterDadosUsuario(
            [FromServices] ICasoDeUsoObterDadosUsuarioInscricao casoDeUsoObterDadosUsuarioInscricao)
        {
            return Ok(await casoDeUsoObterDadosUsuarioInscricao.Executar());
        }
    }
}
