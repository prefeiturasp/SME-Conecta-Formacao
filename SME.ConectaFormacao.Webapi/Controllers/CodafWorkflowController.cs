using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    [Route("api/v1/CodafListaPresenca")]
    public class CodafWorkflowController(ICasoDeUsoEnviarParaDfCodafListaPresenca casoDeUsoEnviarParaDfCodafListaPresenca,
        ICasoDeUsoDevolverParaCorrecaoCodafListaPresenca casoDeUsoDevolverParaCorrecaoCodafListaPresenca) : BaseController
    {

        [HttpPatch("{codafListaPresencaId:long}/enviar-para-df")]
        public async Task<IActionResult> EnviarParaDf(long codafListaPresencaId)
        {
            var resultado = await casoDeUsoEnviarParaDfCodafListaPresenca.ExecutarAsync(codafListaPresencaId);
            return ProcessarResultado(resultado);
        }

        [HttpPatch("{codafListaPresencaId:long}/devolver-para-correcao")]
        public async Task<IActionResult> DevolverParaCorrecao(long codafListaPresencaId, [FromBody] string justificativa)
        {
            var resultado = await casoDeUsoDevolverParaCorrecaoCodafListaPresenca.ExecutarAsync(codafListaPresencaId, justificativa);
            return ProcessarResultado(resultado);
        }
    }
}