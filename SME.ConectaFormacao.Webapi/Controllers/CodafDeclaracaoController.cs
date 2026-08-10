using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class CodafDeclaracaoController(
        ICasoDeUsoEmitirDeclaracaoCodaf casoDeUsoEmitirDeclaracaoCodaf) : BaseController
    {
        [HttpPost("{codafNaoHomologadoId:long}/emitir")]
        [ProducesResponseType(typeof(Resultado), 200)]
        [ProducesResponseType(typeof(Resultado), 404)]
        public async Task<IActionResult> EmitirDeclaracoesCodaf([FromRoute] long codafNaoHomologadoId)
        {
            var resultado = await casoDeUsoEmitirDeclaracaoCodaf.ExecutarAsync(codafNaoHomologadoId);
            return ProcessarResultado(resultado);
        }  
    }
}
