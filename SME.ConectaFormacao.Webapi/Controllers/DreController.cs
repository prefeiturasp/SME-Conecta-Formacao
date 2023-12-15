using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Dre;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Webapi.Controllers.Filtros;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class DreController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DreDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.AreaPromotora_C, Permissao.AreaPromotora_I, Permissao.AreaPromotora_A, Permissao.AreaPromotora_E, 
            Permissao.Proposta_C, Permissao.Proposta_I, Permissao.Proposta_A, Permissao.Proposta_E, 
            Policy = "Bearer")]
        public async Task<IActionResult> ObterListaDre(
            [FromServices] ICasoDeUsoObterListaDre useCase,
            [FromQuery] bool exibirOpcaoTodos = false)
        {
            return Ok(await useCase.Executar(exibirOpcaoTodos));
        }
    }
}