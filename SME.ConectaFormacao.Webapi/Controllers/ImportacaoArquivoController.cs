using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class ImportacaoArquivoController : BaseController
    {
        [HttpPost("inscricao-cursista")]
        [ProducesResponseType(typeof(RetornoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ImportarArquivoInscricaoCursista([FromBody] ImportacaoArquivoInscricaoDTO importacaoArquivoInscricaoDto, 
            [FromServices] ICasoDeUsoImportacaoArquivoInscricaoCursista casoDeUsoImportacaoArquivoInscricaoCursista)
        {
            return Ok(await casoDeUsoImportacaoArquivoInscricaoCursista.Executar(importacaoArquivoInscricaoDto));
        }
    }
}
