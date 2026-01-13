using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    [Route("api/v1/codaf-lista-presenca")]
    public class CodafArquivoController(
        ICasoDeUsoObterModeloTermoResponsabilidadeCodaf casoDeUsoObterModeloTermoResponsabilidadeCodaf,
        ICasoDeUsoUploadAnexoTemporarioCodafListaPresenca casoDeUsoUploadAnexoTemporarioCodafListaPresenca) : BaseController
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
    }
}
