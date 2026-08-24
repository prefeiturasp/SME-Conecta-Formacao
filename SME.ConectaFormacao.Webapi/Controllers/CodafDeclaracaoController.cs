using Microsoft.AspNetCore.Authorization;
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
        ICasoDeUsoListarTodasDeclaracoesCodaf casoDeUsoListarTodasDeclaracoesCodaf) : BaseController
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
    }
}
