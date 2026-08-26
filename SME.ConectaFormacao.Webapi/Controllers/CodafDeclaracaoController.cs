using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Webapi.Controllers.Filtros;
using SME.ConectaFormacao.Webapi.Filtros;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    [PadronizarRetornoFiltro]
    public class CodafDeclaracaoController(
        ICasoDeUsoEmitirDeclaracaoCodaf casoDeUsoEmitirDeclaracaoCodaf,
        ICasoDeUsoListarMinhasDeclaracoesCodaf casoDeUsoListarMinhasDeclaracoesCodaf,
        ICasoDeUsoObterDeclaracaoCodafParaDownload casoDeUsoObterDeclaracaoCodafParaDownload,
        ICasoDeUsoListarTodasDeclaracoesCodaf casoDeUsoListarTodasDeclaracoesCodaf,
        ICasoDeUsoDownloadLoteDeclaracoes casoDeUsoDownloadLoteDeclaracoes) : BaseController
    {
        [HttpPost("{codafNaoHomologadoId:long}/emitir")]
        [ProducesResponseType(typeof(Resultado), 200)]
        [ProducesResponseType(typeof(Resultado), 404)]
        public async Task<IActionResult> EmitirDeclaracoesCodaf([FromRoute] long codafNaoHomologadoId)
        {
            var resultado = await casoDeUsoEmitirDeclaracaoCodaf.ExecutarAsync(codafNaoHomologadoId);
            return ProcessarResultado(resultado);
        }

        [HttpGet("minhas")]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<MinhasDeclaracoesCodafDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<MinhasDeclaracoesCodafDto>>), 404)]
        public async Task<IActionResult> ListarMinhasDeclaracoes([FromQuery] FiltroListaMinhasDeclaracoesCodafDto filtro)
        {
            var resultado = await casoDeUsoListarMinhasDeclaracoesCodaf.ExecutarAsync(filtro);
            return ProcessarResultado(resultado);
        }

        [HttpGet("{declaracaoCodafId}/download")]
        [ProducesResponseType(typeof(Resultado<CodafDeclaracaoParaDownloadDto>), 200)]
        [ProducesResponseType(typeof(Resultado<CodafDeclaracaoParaDownloadDto>), 422)]
        public async Task<IActionResult> ObterDeclaracaoParaDownload(long declaracaoCodafId)
        {
            var resultado = await casoDeUsoObterDeclaracaoCodafParaDownload.ExecutarAsync(declaracaoCodafId);
            return ProcessarResultado(resultado);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<ListagemDeclaracoesCodafDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<ListagemDeclaracoesCodafDto>>), 404)]
        [Permissao(Permissao.Codaf_I, Policy = "Bearer")]
        public async Task<IActionResult> ListarTodasDeclaracoes([FromQuery] FiltroListagemTodasDeclaracoesCodafDto filtro)
        {
            var resultado = await casoDeUsoListarTodasDeclaracoesCodaf.ExecutarAsync(filtro);
            return ProcessarResultado(resultado);
        }

        [HttpPost("download-lote")]
        [ProducesResponseType(typeof(FileStreamResult), 200)]
        [ProducesResponseType(typeof(Resultado), 404)]
        [Permissao(Permissao.Codaf_I, Policy = "Bearer")]
        public async Task DownloadLoteDeclaracoes([FromBody] List<long> ids, CancellationToken cancellationToken)
        {
            var syncIoFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
            syncIoFeature?.AllowSynchronousIO = true;
            var dataHoraAtual = DateTime.Now.ToString("ddMMyyyy_HHmmss");
            var nomeArquivo = $"DECLARACOES_{dataHoraAtual}.zip";

            Response.ContentType = "application/zip";
            Response.Headers.Append("Content-Disposition", $"attachment; filename={nomeArquivo}");

            await casoDeUsoDownloadLoteDeclaracoes.ExecutarAsync(ids, Response.Body, cancellationToken);
        }
    }
}
