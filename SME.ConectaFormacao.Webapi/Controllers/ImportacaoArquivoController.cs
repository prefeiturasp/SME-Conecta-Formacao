using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Webapi.Controllers.Filtros;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

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

        [HttpGet("{propostaId}/arquivos-importados")]
        [ProducesResponseType(typeof(PaginacaoResultadoDTO<ArquivoInscricaoImportadoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Inscricao_C, Policy = "Bearer")]
        public async Task<IActionResult> ObterArquivosImportados([FromRoute] long propostaId, [FromServices] ICasoDeUsoObterArquivosInscricaoImportados useCase)
        {
            return Ok(await useCase.Executar(propostaId));
        }

        [HttpGet("{arquivoId}/registros-inconsistencia")]
        [ProducesResponseType(typeof(PaginacaoResultadoDTO<RegistroDaInscricaoInsconsistenteDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Inscricao_C, Policy = "Bearer")]
        public async Task<IActionResult> ObterRegistrosComInconsistencia([FromRoute] long arquivoId, [FromServices] ICasoDeUsoObterRegistrosDaIncricaoInconsistentes useCase)
        {
            return Ok(await useCase.Executar(arquivoId));
        }


        [HttpPost("{arquivoImportacaoId}/continuar")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ContinuarProcessamentoArquivo([FromRoute] long arquivoImportacaoId, [FromServices] ICasoDeUsoInscricaoManualContinuarProcessamento useCase)
        {
            return Ok(await useCase.Executar(arquivoImportacaoId));
        }

        [HttpPost("{arquivoImportacaoId}/cancelar")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> CancelarProcessamentoArquivo([FromRoute] long arquivoImportacaoId, [FromServices] ICasoDeUsoInscricaoManualCancelarProcessamento useCase)
        {
            return Ok(await useCase.Executar(arquivoImportacaoId));
        }
    }
}
