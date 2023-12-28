using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class InscricaoController : BaseController
    {
        [HttpGet("dados-inscricao")]
        [ProducesResponseType(typeof(DadosInscricaoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterDadosUsuario(
            [FromServices] ICasoDeUsoObterDadosInscricao casoDeUsoObterDadosInscricao)
        {
            return Ok(await casoDeUsoObterDadosInscricao.Executar());
        }

        [HttpPost]
        [ProducesResponseType(typeof(DadosInscricaoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> SalvarInscricao(
            [FromServices] ICasoDeUsoSalvarInscricao casoDeUsoSalvarInscricao,
            [FromBody] InscricaoDTO inscricaoDTO)
        {
            return Ok(await casoDeUsoSalvarInscricao.Executar(inscricaoDTO));
        }
    }
}
