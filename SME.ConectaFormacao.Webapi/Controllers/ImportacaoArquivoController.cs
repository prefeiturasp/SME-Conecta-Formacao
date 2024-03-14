using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Webapi.Controllers.Filtros;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class ImportacaoArquivoController : BaseController
    {
        [HttpPost("inscricao-cursista")]
        [ProducesResponseType(typeof(RetornoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Inscricao_I, Policy = "Bearer")]
        public async Task<IActionResult> ImportarArquivoInscricaoCursista(IFormFile arquivo, long propostaId,
            [FromServices] ICasoDeUsoImportacaoArquivoInscricaoCursista casoDeUsoImportacaoArquivoInscricaoCursista)
        {
            return Ok(await casoDeUsoImportacaoArquivoInscricaoCursista.Executar(arquivo, propostaId));
        }
    }
}
