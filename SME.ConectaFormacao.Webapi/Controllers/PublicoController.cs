using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Formacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.PalavraChave;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    public class PublicoController : BaseController
    {
        [HttpGet("cargo-funcao/tipo/{tipo}")]
        [ProducesResponseType(typeof(IEnumerable<CargoFuncaoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterListaCargoFuncao(
            [FromServices] ICasoDeUsoObterCargoFuncao casoDeUsoObterCargoFuncao,
            [FromRoute] CargoFuncaoTipo? tipo,
            [FromQuery] bool exibirOpcaoOutros = false)
        {
            return Ok(await casoDeUsoObterCargoFuncao.Executar(tipo, exibirOpcaoOutros));
        }

        [HttpGet("area-promotora")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterListaAreaPromotora(
           [FromServices] ICasoDeUsoObterAreaPromotoraListaAreaPublica casoDeUsoObterAreaPromotoraListaAreaPublica)
        {
            return Ok(await casoDeUsoObterAreaPromotoraListaAreaPublica.Executar());
        }

        [HttpGet("palavra-chave")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterListaPalavraChave(
            [FromServices] ICasoDeUsoObterPalavraChave casoDeUsoObterPalavraChave)
        {
            return Ok(await casoDeUsoObterPalavraChave.Executar());
        }

        [HttpGet("formato")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterListaFormato(
            [FromServices] ICasoDeUsoObterTodosFormatos casoDeUsoObterTodosFormatos)
        {
            return Ok(await casoDeUsoObterTodosFormatos.Executar());
        }

        [HttpGet("formacao-listagem")]
        [ProducesResponseType(typeof(PaginacaoResultadoDTO<IEnumerable<RetornoListagemFormacaoDTO>>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterListagemFormacao([FromQuery] FiltroListagemFormacaoDTO filtroListagemFormacaoDTO, [FromServices] ICasoDeUsoObterListagemFormacaoPaginada casoDeUsoObterListagemFormacaoPaginada)
        {
            return Ok(await casoDeUsoObterListagemFormacaoPaginada.Executar(filtroListagemFormacaoDTO));
        }

        [HttpGet("formacao-detalhada/{propostaId}")]
        [ProducesResponseType(typeof(RetornoFormacaoDetalhadaDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterFormacaoDetalhada([FromRoute] long propostaId, [FromServices] ICasoDeUsoObterFormacaoDetalhada casoDeUsoObterFormacaoDetalhada)
        {
            return Ok(await casoDeUsoObterFormacaoDetalhada.Executar(propostaId));
        }
    }
}