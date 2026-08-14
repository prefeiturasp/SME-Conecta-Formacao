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
    public class CodafArquivoController(
        ICasoDeUsoObterModeloTermoResponsabilidadeCodaf casoDeUsoObterModeloTermoResponsabilidadeCodaf,
        ICasoDeUsoUploadAnexoTemporarioCodafListaPresenca casoDeUsoUploadAnexoTemporarioCodafListaPresenca,
        ICasoDeUsoGerarArquivoRemessaConclusaoCodaf casoDeUsoGerarArquivoRemessaConclusaoCodaf) : BaseController
    {

        [HttpGet("termo-responsabilidade/modelo")]
        [ProducesResponseType(typeof(Resultado<ArquivoDto>), 200)]
        [ProducesResponseType(typeof(Resultado<ArquivoDto>), 404)]
        public async Task<IActionResult> ObterModeloTermoResponsabilidade()
        {
            var resultado = casoDeUsoObterModeloTermoResponsabilidadeCodaf.Executar();

            if (resultado.Sucesso)
                return File(resultado.Dados!.Stream, resultado.Dados.ContentType, resultado.Dados.Nome);

            return ProcessarResultado(resultado);
        }

        [HttpPost("anexos/temporarios")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Resultado<CodafAnexoTemporarioDto>), 201)]
        [ProducesResponseType(typeof(Resultado<CodafAnexoTemporarioDto>), 404)]
        public async Task<IActionResult> UploadAnexoTemporario([FromForm] IFormFile arquivo)
        {
            var resultado = await casoDeUsoUploadAnexoTemporarioCodafListaPresenca.ExecutarAsync(arquivo);
            return ProcessarResultado(resultado);
        }

        [HttpPost("{codafListaPresencaId}/gerar-remessa-conclusao")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(Resultado<ArquivoDto>), 404)]
        public async Task<IActionResult> GerarArquivoRemessaConclusaoCodaf(long codafListaPresencaId)
        {
            var resultado = await casoDeUsoGerarArquivoRemessaConclusaoCodaf.ExecutarAsync(codafListaPresencaId);
            if (resultado.Sucesso)
                return File(resultado.Dados!.Stream, resultado.Dados.ContentType, resultado.Dados.Nome);
            return ProcessarResultado(resultado);
        }
    }
}
