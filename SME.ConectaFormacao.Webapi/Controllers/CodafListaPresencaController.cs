using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Webapi.Filtros;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    [PadronizarRetornoFiltro]
    [Route("api/v1/CodafListaPresenca")]
    public class CodafListaPresencaController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(Resultado<CodafListaPresencaDto>), 201)]
        [ProducesResponseType(typeof(Resultado<CodafListaPresencaDto>), 400)]
        [ProducesResponseType(typeof(Resultado<CodafListaPresencaDto>), 404)]
        [ProducesResponseType(typeof(Resultado<CodafListaPresencaDto>), 422)]
        public async Task<IActionResult> Cadastrar(
            [FromBody] CodafListaPresencaCadastroDto codafListaPresencaCadastroDto, 
            [FromServices] ICasoDeUsoCriarCodafListaPresenca casoDeUsoCriarCodafListaPresenca)
        {
            var resultado = await casoDeUsoCriarCodafListaPresenca.ExecutarAsync(codafListaPresencaCadastroDto);
            return ProcessarCriado(resultado);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(Resultado), 204)]
        [ProducesResponseType(typeof(Resultado), 400)]
        [ProducesResponseType(typeof(Resultado), 404)]
        [ProducesResponseType(typeof(Resultado), 422)]
        public async Task<IActionResult> Atualizar(
            int id, [FromBody] CodafListaPresencaEdicaoDto codafListaPresencaEdicaoDto,
            [FromServices] ICasoDeUsoAtualizarCodafListaPresenca casoDeUsoAtualizar)
        {
            var resultado = await casoDeUsoAtualizar.ExecutarAsync(codafListaPresencaEdicaoDto, id);
            return ProcessarResultado(resultado);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<ListaPresencaCodafResumoDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<ListaPresencaCodafResumoDto>>), 400)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<ListaPresencaCodafResumoDto>>), 500)]
        public async Task<IActionResult> ObterListaPaginada(
            [FromQuery] FiltroListaPresencaCodafDto filtro,
            [FromServices] ICasoDeUsoListarCodafListaPresenca casoDeUsoListarCodafListaPresenca)
        {
            var resultado = await casoDeUsoListarCodafListaPresenca.ExecutarAsync(filtro);
            return ProcessarResultado(resultado);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(Resultado<CodafListaPresencaDto>), 200)]
        [ProducesResponseType(typeof(Resultado<CodafListaPresencaDto>), 404)]
        public async Task<IActionResult> ObterPorId(long id,
            [FromServices] ICasoDeUsoObterCodafListaPresencaPorId casoDeUsoObterCodafListaPresencaPorId)
        {
            var resultado = await casoDeUsoObterCodafListaPresencaPorId.ExecutarAsync(id);
            return ProcessarResultado(resultado);
        }

        [HttpDelete("retificacoes/{retificacaoId:long}")]
        [ProducesResponseType(typeof(Resultado<bool>), 204)]
        [ProducesResponseType(typeof(Resultado<bool>), 404)]
        public async Task<IActionResult> RemoverRetificacao(long retificacaoId,
            [FromServices] ICasoDeUsoRemoverCodafRetificacaoListaPresenca casoDeUsoRemoverCodafRetificacaoListaPresenca)
        {
            var resultado = await casoDeUsoRemoverCodafRetificacaoListaPresenca.ExecutarAsync(retificacaoId);
            return ProcessarResultado(resultado);
        }

        [HttpDelete("{codafListaPresencaId:long}")]
        public async Task<IActionResult> Excluir(long codafListaPresencaId,
            [FromServices] ICasoDeUsoExcluirCodafListaPresenca casoDeUsoExcluirCodafListaPresenca)
        {
            var resultado = await casoDeUsoExcluirCodafListaPresenca.ExecutarAsync(codafListaPresencaId);
            return ProcessarResultado(resultado);
        }

        [HttpPost("{codafId:long}/imprimir")]
        [ProducesResponseType(typeof(Resultado<ArquivoDto>), 200)]
        [ProducesResponseType(typeof(Resultado<ArquivoDto>), 404)]
        public async Task<IActionResult> ImprimirRelatorioCodafAsync(long codafId,
            [FromServices] ICasoDeUsoGerarRelatorioCodaf casoDeUsoGerarRelatorioCodaf)
        {
            var resultado = await casoDeUsoGerarRelatorioCodaf.ExecutarAsync(codafId);

            if (resultado.Sucesso)
                return File(resultado.Dados!.Stream, resultado.Dados.ContentType, resultado.Dados.Nome);

            return ProcessarResultado(resultado);
        }

        [HttpPatch("{codafId:long}/inscritos")]
        [ProducesResponseType(typeof(Resultado), 204)]
        [ProducesResponseType(typeof(Resultado), 400)]
        public async Task<IActionResult> SalvarInscritosAsync(
            [FromRoute] long codafId, 
            [FromBody] IList<CodafInscritoListaPresencaSalvarDto> inscritos,
            [FromServices] ICasoDeUsoSalvarInscritosCodaf casoDeUsoSalvarInscritosCodaf)
        {
            var resultado = await casoDeUsoSalvarInscritosCodaf.ExecutarAsync(inscritos, codafId);
            return ProcessarResultado(resultado);
        }

        [HttpPatch("{codafId:long}/finalizar")]
        [ProducesResponseType(typeof(Resultado), 204)]
        [ProducesResponseType(typeof(Resultado), 400)]
        public async Task<IActionResult> FinalizarCodafAsync(
           [FromRoute] long codafId,
           [FromServices] ICasoDeUsoFinalizarCodafListaPresenca casoDeUsoFinalizarCodafListaPresenca)
        {
            var resultado = await casoDeUsoFinalizarCodafListaPresenca.ExecutarAsync(codafId);
            return ProcessarResultado(resultado);
        }
    }
}