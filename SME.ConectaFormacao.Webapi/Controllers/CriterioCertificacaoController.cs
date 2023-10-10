using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.CriterioCertificacao;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    //[Authorize("Bearer")]
    public class CriterioCertificacaoController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterCriterioCertificacao([FromServices]ICasoDeUsoCriterioCertificacao useCase)
        {
            return Ok(await useCase.Executar());
        }
    }
}