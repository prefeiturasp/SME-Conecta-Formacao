using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class UsuarioController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(UsuarioExternoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 601)]
        public async Task<IActionResult> Inserir(UsuarioExternoDTO usuarioExternoDto, [FromServices] ICasoDeUsoInserirUsuarioExterno usoInserirUsuario)
        {
            return Ok(await usoInserirUsuario.InserirUsuarioExterno(usuarioExternoDto));
        }

        [HttpPost("{login}/solicitar-recuperacao-senha")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [AllowAnonymous]
        public async Task<IActionResult> SolicitarRecuperacaoSenha([FromRoute] string login, [FromServices] ICasoDeUsoUsuarioSolicitarRecuperacaoSenha casoDeUsoUsuarioSolicitarRecuperacaoSenha)
        {
            return Ok(await casoDeUsoUsuarioSolicitarRecuperacaoSenha.Executar(login));
        }

        [HttpGet("valida-token-recuperacao-senha/{token}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [AllowAnonymous]
        public async Task<IActionResult> TokenRecuperacaoSenhaEstaValido([FromRoute] Guid token, [FromServices] ICasoDeUsoUsuarioValidacaoSenhaToken casoDeUsoUsuarioValidacaoSenhaToken)
        {
            return Ok(await casoDeUsoUsuarioValidacaoSenhaToken.Executar(token));
        }

        [HttpGet("validar-email/{token}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [AllowAnonymous]
        public async Task<IActionResult> ValidarEmailToken([FromRoute] Guid token, [FromServices] ICasoDeUsoUsuarioValidacaoEmailToken casoDeUsoUsuarioValidacaoEmailToken)
        {
            return Ok(await casoDeUsoUsuarioValidacaoEmailToken.Executar(token));
        }

        [HttpPut("recuperar-senha")]
        [ProducesResponseType(typeof(UsuarioPerfisRetornoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [AllowAnonymous]
        public async Task<IActionResult> RecuperarSenha([FromBody] RecuperacaoSenhaDto recuperacaoSenhaDto, [FromServices] ICasoDeUsoUsuarioRecuperarSenha casoDeUsoUsuarioRecuperarSenha)
        {
            return Ok(await casoDeUsoUsuarioRecuperarSenha.Executar(recuperacaoSenhaDto));
        }

        [HttpGet("{login}")]
        [ProducesResponseType(typeof(DadosUsuarioDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> MeusDados([FromRoute] string login, [FromServices] ICasoDeUsoUsuarioMeusDados casoDeUsoUsuarioMeusDados)
        {
            return Ok(await casoDeUsoUsuarioMeusDados.Executar(login));
        }

        [HttpPut("{login}/senha")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> AlterarSenha([FromRoute] string login, [FromBody] AlterarSenhaUsuarioDTO alterarSenhaUsuarioDto, [FromServices] ICasoDeUsoUsuarioAlterarSenha casoDeUsoUsuarioAlterarSenha)
        {
            return Ok(await casoDeUsoUsuarioAlterarSenha.Executar(login, alterarSenhaUsuarioDto));
        }

        [HttpPut("{login}/email")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> AlterarEmailCoreSSO([FromRoute] string login, [FromBody] EmailUsuarioDTO emailUsuarioDto, [FromServices] ICasoDeUsoUsuarioAlterarEmail casoDeUsoUsuarioAlterarEmail)
        {
            return Ok(await casoDeUsoUsuarioAlterarEmail.Executar(login, emailUsuarioDto.Email));
        }

        [HttpGet("{login}/reenviar-email")]
        [ProducesResponseType(typeof(DadosUsuarioDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [AllowAnonymous]
        public async Task<IActionResult> ReenviarEmailParaValidacao([FromRoute] string login, [FromServices] ICasoDeUsoReenviarEmail casoDeUsoReenviarEmail)
        {
            return Ok(await casoDeUsoReenviarEmail.Executar(login));
        }

        [HttpPut("{login}/nome")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> AlterarNomeConectaECoreSSO([FromRoute] string login, [FromBody] NomeUsuarioDTO nomeUsuarioDto, [FromServices] ICasoDeUsoUsuarioAlterarNome casoDeUsoUsuarioAlterarNome)
        {
            return Ok(await casoDeUsoUsuarioAlterarNome.Executar(login, nomeUsuarioDto.Nome));
        }

        [HttpPut("{login}/unidade-eol")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> AlterarUnidadeEol([FromRoute] string login, [FromBody] UnidadeEolUsuarioDTO unidadeEolUsuarioDTO, [FromServices] ICasoDeUsoUsuarioAlterarUnidadeEol casoDeUsoUsuarioAlterarUnidadeEol)
        {
            return Ok(await casoDeUsoUsuarioAlterarUnidadeEol.Executar(login, unidadeEolUsuarioDTO.CodigoEolUnidade));
        }
    }
}