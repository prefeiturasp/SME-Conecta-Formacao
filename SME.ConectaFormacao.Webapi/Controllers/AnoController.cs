using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Ano;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ano;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Webapi.Controllers.Filtros;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class AnoController : BaseController
    {
        [HttpGet("ano-letivo/{anoLetivo}")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemTodosDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Proposta_I, Permissao.Proposta_A, Permissao.Proposta_E, Policy = "Bearer")]
        public async Task<IActionResult> ObterAnoPorModalidadeAnoLetivo(
            [FromServices] ICasoDeUsoObterListaAnoTurma casoDeUsoObterAnoTurma,
            [FromQuery] FiltroAnoTurmaDTO filtroAnoTurmaDTO 
            )
        {
            return Ok(await casoDeUsoObterAnoTurma.Executar(filtroAnoTurmaDTO));
        }
    }
}
