using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class AreaPromotoraController : BaseController
    {
        [HttpGet("tipos")]
        [ProducesResponseType(typeof(IEnumerable<AreaPromotoraTipoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> ObterTiposAreaPromotora(
            [FromServices] ICasoDeUsoObterTiposAreaPromotora casoDeUsoObterTiposAreaPromotora)
        {
            return Ok(await casoDeUsoObterTiposAreaPromotora.Executar());
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginacaoResultadoDTO<AreaPromotoraPaginadaDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> ObterAreaPromotoraPaginada(
            [FromServices] ICasoDeUsoObterAreaPromotoraPaginada casoDeUsoObterAreaPromotoraPaginada,
            [FromQuery] AreaPromotoraFiltrosDTO filtrosAreaPromotoraDTO)
        {
            return Ok(await casoDeUsoObterAreaPromotoraPaginada.Executar(filtrosAreaPromotoraDTO));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AreaPromotoraCompletoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> ObterAreaPromotoraPorId(
            [FromServices] ICasoDeUsoObterAreaPromotoraPorId casoDeUsoObterAreaPromotoraPorId,
            [FromRoute] long id)
        {
            return Ok(await casoDeUsoObterAreaPromotoraPorId.Executar(id));
        }

        [HttpPost]
        [ProducesResponseType(typeof(long), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> InserirAreaPromotora(
            [FromServices] ICasoDeUsoInserirAreaPromotora casoDeUsoInserirAreaPromotora,
            [FromBody] AreaPromotoraDTO areaPromotoraDTO)
        {
            return Ok(await casoDeUsoInserirAreaPromotora.Executar(areaPromotoraDTO));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> AlterarAreaPromotora(
            [FromServices] ICasoDeUsoAlterarAreaPromotora casoDeUsoAlterarAreaPromotora,
            [FromRoute] long id,
            [FromBody] AreaPromotoraDTO areaPromotoraDTO)
        {
            return Ok(await casoDeUsoAlterarAreaPromotora.Executar(id, areaPromotoraDTO));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Authorize("Bearer")]
        public async Task<IActionResult> RemoverAreaPromotora(
            [FromServices] ICasoDeUsoRemoverAreaPromotora casoDeUsoRemoverAreaPromotora,
            [FromRoute] long id)
        {
            return Ok(await casoDeUsoRemoverAreaPromotora.Executar(id));
        }
    }
}
