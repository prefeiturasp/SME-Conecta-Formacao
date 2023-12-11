using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class AutenticacaoController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [ProducesResponseType(typeof(UsuarioPerfisRetornoDTO), 200)]
        [AllowAnonymous]
        public async Task<IActionResult> Autenticar(
            [FromServices] ICasoDeUsoAutenticarUsuario casoDeUsoAutenticar,
            AutenticacaoDTO autenticacaoDto)
        {
            return Ok(await casoDeUsoAutenticar.Executar(autenticacaoDto));
        }

        [HttpPost("revalidar")]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [ProducesResponseType(typeof(UsuarioPerfisRetornoDTO), 200)]
        [AllowAnonymous]
        public async Task<IActionResult> Revalidar(
            [FromServices] ICasoDeUsoAutenticarRevalidar casoDeUsoAutenticarRevalidar,
            [FromBody] AutenticacaoRevalidarDTO autenticacaoRevalidarDTO)
        {
            return Ok(await casoDeUsoAutenticarRevalidar.Executar(autenticacaoRevalidarDTO.Token));
        }

        [HttpPut("perfis/{perfilUsuarioId}")]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [ProducesResponseType(typeof(UsuarioPerfisRetornoDTO), 200)]
        [Authorize("Bearer")]
        public async Task<IActionResult> AtualizarPerfil(
            [FromServices] ICasoDeUsoAutenticarAlterarPerfil casoDeUsoAutenticarAlterarPerfil,
            Guid perfilUsuarioId)
        {
            return Ok(await casoDeUsoAutenticarAlterarPerfil.Executar(perfilUsuarioId));
        }
    }
}
