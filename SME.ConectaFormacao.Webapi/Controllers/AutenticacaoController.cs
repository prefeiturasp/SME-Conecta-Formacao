using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.CasosDeUso;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class AutenticacaoController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 601)]
        [ProducesResponseType(typeof(UsuarioAutenticacaoRetornoDTO), 200)]
        [AllowAnonymous]
        public async Task<IActionResult> Autenticar(AutenticacaoDTO autenticacaoDto, [FromServices] ICasoDeUsoAutenticar casoDeUsoAutenticar)
        {
            var retornoAutenticacao = await casoDeUsoAutenticar.Executar(autenticacaoDto.Login, autenticacaoDto.Senha);
            return Ok(retornoAutenticacao);
        }
    }
}
