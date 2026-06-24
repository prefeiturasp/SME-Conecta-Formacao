using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class CodafSuplementarController : BaseController
    {
        [HttpGet("codaf/{codafId:long}")]
        public async Task<IActionResult> ObterPorCodafIdAsync(long codafId, [FromServices] ICasoDeUsoObterCodafSuplementarPorCodafId casoDeUso)
        {
            var resultado = await casoDeUso.ExecutarAsync(codafId);
            return ProcessarResultado(resultado);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Resultado<CodafSuplementarDetalhadoDto>), 201)]
        [ProducesResponseType(typeof(Resultado<CodafSuplementarDetalhadoDto>), 400)]
        [ProducesResponseType(typeof(Resultado<CodafSuplementarDetalhadoDto>), 404)]
        [ProducesResponseType(typeof(Resultado<CodafSuplementarDetalhadoDto>), 422)]
        public async Task<IActionResult> Cadastrar(
            [FromBody] CodafSuplementarCadastroDto codafSuplementarCadastroDto,
            [FromServices] ICasoDeUsoCriarCodafSuplementar casoDeUsoCriarCodafSuplementar)
        {
            var resultado = await casoDeUsoCriarCodafSuplementar.ExecutarAsync(codafSuplementarCadastroDto);
            return ProcessarCriado(resultado);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafSuplementarResumoDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafSuplementarResumoDto>>), 400)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafSuplementarResumoDto>>), 500)]
        public async Task<IActionResult> ObterListaPaginada(
            [FromQuery] FiltroCodafSuplementarDto filtro,
            [FromServices] ICasoDeUsoListarCodafSuplementar casoDeUsoListarCodafSuplementar)
        {
            var resultado = await casoDeUsoListarCodafSuplementar.ExecutarAsync(filtro);
            return ProcessarResultado(resultado);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(Resultado<CodafSuplementarDto>), 200)]
        [ProducesResponseType(typeof(Resultado<CodafSuplementarDto>), 404)]
        public async Task<IActionResult> ObterPorId(long id,
            [FromServices] ICasoDeUsoObterCodafSuplementarPorId casoDeUsoObterCodafSuplementarPorId)
        {
            var resultado = await casoDeUsoObterCodafSuplementarPorId.ExecutarAsync(id);
            return ProcessarResultado(resultado);
        }
    }
}