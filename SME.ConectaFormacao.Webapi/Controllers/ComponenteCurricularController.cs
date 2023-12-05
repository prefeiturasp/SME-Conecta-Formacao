using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;
using SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Webapi.Controllers.Filtros;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    [Route("api/v1/componentes-curriculares")]
    public class ComponenteCurricularController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemTodosDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Proposta_I, Permissao.Proposta_A, Permissao.Proposta_E, Policy = "Bearer")]
        public async Task<IActionResult> ObterComponentesCurricularesPorModalidadeAnoLetivoAno(
            [FromQuery] ComponenteCurricularFiltrosDto componenteCurricularFiltrosDto,
            [FromServices] ICasoDeUsoObterComponentesCurricularesPorModalidadeAnoLetivoAno casoDeUsoObterComponentesCurricularesPorModalidadeAnoLetivoAno)
        {
            return Ok(await casoDeUsoObterComponentesCurricularesPorModalidadeAnoLetivoAno.Executar(componenteCurricularFiltrosDto));
        }
    }
}
