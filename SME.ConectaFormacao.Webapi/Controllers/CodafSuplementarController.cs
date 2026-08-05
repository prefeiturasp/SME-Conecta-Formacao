using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
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

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(Resultado), 204)]
        [ProducesResponseType(typeof(Resultado), 400)]
        [ProducesResponseType(typeof(Resultado), 404)]
        [ProducesResponseType(typeof(Resultado), 422)]
        public async Task<IActionResult> Atualizar(
            int id, [FromBody] CodafSuplementarCadastroDto codafSuplementarCadastroDto,
            [FromServices] ICasoDeUsoAtualizarCodafSuplementar casoDeUsoAtualizar)
        {
            var resultado = await casoDeUsoAtualizar.ExecutarAsync(codafSuplementarCadastroDto, id);
            return ProcessarResultado(resultado);
        }

        [HttpDelete("{codafSuplementarId:long}")]
        public async Task<IActionResult> Excluir(long codafSuplementarId,
            [FromServices] ICasoDeUsoExcluirCodafSuplementar casoDeUsoExcluirCodafSuplementar)
        {
            var resultado = await casoDeUsoExcluirCodafSuplementar.ExecutarAsync(codafSuplementarId);
            return ProcessarResultado(resultado);
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

        [HttpPost("anexos/temporarios")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Resultado<CodafAnexoTemporarioDto>), 201)]
        [ProducesResponseType(typeof(Resultado<CodafAnexoTemporarioDto>), 404)]
        public async Task<IActionResult> UploadAnexoTemporario(
            [FromForm] IFormFile arquivo,
            [FromServices] ICasoDeUsoUploadAnexoTemporarioCodafSuplementar casoDeUsoUploadAnexoTemporarioCodafSuplementar)
        {
            var resultado = await casoDeUsoUploadAnexoTemporarioCodafSuplementar.ExecutarAsync(arquivo);
            return ProcessarResultado(resultado);
        }

        [HttpDelete("retificacoes/{retificacaoId:long}")]
        [ProducesResponseType(typeof(Resultado<bool>), 204)]
        [ProducesResponseType(typeof(Resultado<bool>), 404)]
        public async Task<IActionResult> RemoverRetificacao(long retificacaoId,
            [FromServices] ICasoDeUsoRemoverCodafSuplementarRetificacao casoDeUsoRemoverCodafSuplementarRetificacao)
        {
            var resultado = await casoDeUsoRemoverCodafSuplementarRetificacao.ExecutarAsync(retificacaoId);
            return ProcessarResultado(resultado);
        }

        [HttpPost("{codafListaPresencaId:long}/imprimir")]
        [ProducesResponseType(typeof(Resultado<ArquivoDto>), 200)]
        [ProducesResponseType(typeof(Resultado<ArquivoDto>), 404)]
        public async Task<IActionResult> ImprimirRelatorioCodafAsync(long codafListaPresencaId,
        [FromServices] ICasoDeUsoGerarRelatorioCodafSuplementar casoDeUsoGerarRelatorioCodafSuplementar)
        {
            var resultado = await casoDeUsoGerarRelatorioCodafSuplementar.ExecutarAsync(codafListaPresencaId);

            if (resultado.Sucesso)
                return File(resultado.Dados!.Stream, resultado.Dados.ContentType, resultado.Dados.Nome);

            return ProcessarResultado(resultado);
        }
    }
}