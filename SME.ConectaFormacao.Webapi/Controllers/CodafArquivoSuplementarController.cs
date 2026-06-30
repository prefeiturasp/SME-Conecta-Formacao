using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    [Route("api/v1/CodafArquivoSuplementar")]
    public class CodafArquivoSuplementarController(
        ICasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar casoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar) : BaseController
    {

        [HttpPost("{codafSuplementarId}/gerar-remessa-conclusao")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(Resultado<CodafSuplementarArquivoDto>), 404)]
        public async Task<IActionResult> GerarArquivoRemessaConclusaoCodafSuplementar(long codafSuplementarId)
        {
            var resultado = await casoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar.ExecutarAsync(codafSuplementarId);
            if (resultado.Sucesso)
                return File(resultado.Dados!.Stream, resultado.Dados.ContentType, resultado.Dados.Nome);
            return ProcessarResultado(resultado);
        }
    }
}
