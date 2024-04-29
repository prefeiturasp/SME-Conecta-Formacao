using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Dtos.Funcionario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Funcionario;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class FuncionarioController : BaseController
    {
        [HttpGet("obter-usuarios-admin-df")]
        [ProducesResponseType(typeof(IEnumerable<RetornoUsuarioLoginNomeDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterUsuariosAdminDf([FromServices] ICasoDeUsoObterUsuariosAdminDf useCase)
        {
            return Ok(await useCase.Executar());
        }

        [HttpGet("obter-parecerista")]
        [ProducesResponseType(typeof(IEnumerable<RetornoUsuarioLoginNomeDTO>),200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterParecerista([FromServices] ICasoDeUsoObterParecerista useCase)
        {
            return Ok(await useCase.Executar());
        }
    }
}