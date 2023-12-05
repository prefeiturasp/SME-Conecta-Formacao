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
    [Route("api/v1/ano")]
    public class AnoController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IdNomeTodosDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Proposta_I, Permissao.Proposta_A, Permissao.Proposta_E, Policy = "Bearer")]
        public async Task<IActionResult> ObterAnoPorModalidadeAnoLetivo(
            [FromQuery] ModalidadeAnoLetivoFiltrosDTO modalidadeAnoLetivoFiltrosDto,
            [FromServices] ICasoDeUsoObterAnosPorModalidadeAnoLetivo casoDeUsoObterAnosPorModalidadeAnoLetivo)
        {
            return Ok(await casoDeUsoObterAnosPorModalidadeAnoLetivo.Executar(modalidadeAnoLetivoFiltrosDto));
        }
    }
}
