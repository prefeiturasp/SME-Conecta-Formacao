using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Dtos.FuncionarioExterno;
using SME.ConectaFormacao.Aplicacao.Interfaces.FuncionarioExterno.ObterFuncionarioExternoPorCpf;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class FuncionarioExternoController : BaseController
    {
        [HttpGet("{cpf}")]
        [ProducesResponseType(typeof(FuncionarioExternoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterFuncionarioExternoPorCfp(
            [FromRoute] string cpf,
            [FromServices] ICasoDeUsoObterFuncionarioExternoPorCpf useCase
            )
        {
            return Ok(await useCase.Executar(cpf));
        }
    }
}