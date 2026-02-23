using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Webapi.Controllers.Filtros;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class CodafCertificadoController(
        ICasoDeUsoEmitirCertificadoCodaf casoDeUsoEmitirCertificadoCodaf,
        ICasoDeUsoListarMeusCertificadosCodaf casoDeUsoListarMeusCertificadosCodaf,
        ICasoDeUsoObterCertificadoCodafParaDownload casoDeUsoObterCertificadoCodafParaDownload,
        ICasoDeUsoListarTodosCertificadosCodaf casoDeUsoListarTodosCertificadosCodaf) : BaseController
    {
        [HttpPost("{codafListaPresencaId}/emitir-certificados")]
        [ProducesResponseType(typeof(Resultado), 200)]
        [ProducesResponseType(typeof(Resultado), 404)]
        public async Task<IActionResult> EmitirCertificadosCodaf(long codafListaPresencaId)
        {
            var resultado = await casoDeUsoEmitirCertificadoCodaf.ExecutarAsync(codafListaPresencaId);
            return ProcessarResultado(resultado);
        }

        [HttpGet("meus")]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<MeusCertificadosCodafDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<MeusCertificadosCodafDto>>), 404)]
        public async Task<IActionResult> ListarMeusCertificados([FromQuery] FiltroListaMeusCertificadosCodafDto filtro)
        {
            var resultado = await casoDeUsoListarMeusCertificadosCodaf.ExecutarAsync(filtro);
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

        [HttpGet]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<ListagemCertificadosCodafDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<ListagemCertificadosCodafDto>>), 404)]
        [Permissao(Permissao.Codaf_I, Policy = "Bearer")]
        public async Task<IActionResult> ListarTodosCertificados([FromQuery] FiltroListaTodosCertificadosCodafDto filtro)
        {
            var resultado = await casoDeUsoListarTodosCertificadosCodaf.ExecutarAsync(filtro);
            return ProcessarResultado(resultado);
        }
    }
}
