using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class CodafCertificadoController(
        ICasoDeUsoEmitirCertificadoCodaf casoDeUsoEmitirCertificadoCodaf,
        ICasoDeUsoListarCertificadoCodafUsuario casoDeUsoListarCertificadoCodafUsuario,
        ICasoDeUsoObterCertificadoCodafParaDownload casoDeUsoObterCertificadoCodafParaDownload) : BaseController
    {
        [HttpPost("{codafListaPresencaId}/emitir-certificados")]
        [ProducesResponseType(typeof(Resultado), 200)]
        [ProducesResponseType(typeof(Resultado), 404)]
        public async Task<IActionResult> EmitirCertificadosCodaf(long codafListaPresencaId)
        {
            var resultado = await casoDeUsoEmitirCertificadoCodaf.ExecutarAsync(codafListaPresencaId);
            return ProcessarResultado(resultado);
        }

        [HttpGet("certificados-usuario")]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<ListagemResultadoCertificadoCodafDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<ListagemResultadoCertificadoCodafDto>>), 404)]
        public async Task<IActionResult> ListarCertificadosUsuario([FromQuery] FiltroListaCertificadoCodafDto filtro)
        {
            var resultado = await casoDeUsoListarCertificadoCodafUsuario.ExecutarAsync(filtro);
            return ProcessarResultado(resultado);
        }

        [HttpGet("{certificadoCodafId}/download")]
        [ProducesResponseType(typeof(Resultado<CodafCertificadoParaDownloadDto>), 200)]
        [ProducesResponseType(typeof(Resultado<CodafCertificadoParaDownloadDto>), 404)]
        public async Task<IActionResult> ObterCertificadoParaDownload(long certificadoCodafId)
        {
            var resultado = await casoDeUsoObterCertificadoCodafParaDownload.ExecutarAsync(certificadoCodafId);
            return ProcessarResultado(resultado);
        }
    }
}
