using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Webapi.Controllers.Filtros;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class UsuarioRedeParceriaController : BaseController
    {
        [HttpGet("situacao")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.RedeParceria_C, Permissao.RedeParceria_I, Permissao.RedeParceria_A, Permissao.RedeParceria_E, Policy = "Bearer")]
        public async Task<IActionResult> ObterSituacao(
            [FromServices] ICasoDeUsoObterSituacaoUsuarioRedeParceria useCase)
        {
            return Ok(await useCase.Executar());
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginacaoResultadoDTO<UsuarioRedeParceriaPaginadoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.RedeParceria_C, Permissao.RedeParceria_I, Permissao.RedeParceria_A, Permissao.RedeParceria_E, Policy = "Bearer")]
        public async Task<IActionResult> ObterUsuarioRedeParceria(
            [FromServices] ICasoDeUsoObterUsuarioRedeParceriaPaginada casoDeUso,
            [FromQuery] FiltroUsuarioRedeParceriaDTO filtroUsuarioRedeParceriaDTO)
        {
            return Ok(await casoDeUso.Executar(filtroUsuarioRedeParceriaDTO));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UsuarioRedeParceriaDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.RedeParceria_C, Permissao.RedeParceria_I, Permissao.RedeParceria_A, Permissao.RedeParceria_E, Policy = "Bearer")]
        public async Task<IActionResult> ObterUsuarioRedeParceriaPorId(
            [FromServices] ICasoDeUsoObterUsuarioRedeParceriaPorId casoDeUso,
            [FromRoute] long id)
        {
            return Ok(await casoDeUso.Executar(id));
        }

        [HttpPost]
        [ProducesResponseType(typeof(RetornoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.RedeParceria_I, Policy = "Bearer")]
        public async Task<IActionResult> InserirUsuarioRedeParceria(
            [FromServices] ICasoDeUsoInserirUsuarioRedeParceria casoDeUso,
            [FromBody] UsuarioRedeParceriaDTO usuarioRedeParceriaDTO)
        {
            return Ok(await casoDeUso.Executar(usuarioRedeParceriaDTO));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RetornoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.RedeParceria_A, Policy = "Bearer")]
        public async Task<IActionResult> AlterarUsuarioRedeParceria(
            [FromServices] ICasoDeUsoAlterarUsuarioRedeParceria casoDeUso,
            [FromRoute] long id,
            [FromBody] UsuarioRedeParceriaDTO usuarioRedeParceriaDTO)
        {
            return Ok(await casoDeUso.Executar(id, usuarioRedeParceriaDTO));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(RetornoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.RedeParceria_E, Policy = "Bearer")]
        public async Task<IActionResult> RemoverUsuarioRedeParceria(
            [FromServices] ICasoDeUsoRemoverUsuarioRedeParceria casoDeUso,
            [FromRoute] long id)
        {
            return Ok(await casoDeUso.Executar(id));
        }
    }
}
