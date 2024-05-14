using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
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

        [HttpGet]
        [ProducesResponseType(typeof(PaginacaoResultadoDTO<UsuarioRedeParceriaPaginadoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        //[Permissao(Permissao.Usu, Permissao.AreaPromotora_I, Permissao.AreaPromotora_A, Permissao.AreaPromotora_E, Policy = "Bearer")]
        public async Task<IActionResult> ObterUsuarioRedeParceria(
            [FromServices] ICasoDeUsoObterUsuarioRedeParceriaPaginada casoDeUso,
            [FromQuery] FiltroUsuarioRedeParceriaDTO filtroUsuarioRedeParceriaDTO)
        {
            return Ok(await casoDeUso.Executar(filtroUsuarioRedeParceriaDTO));
        }
    }
}
