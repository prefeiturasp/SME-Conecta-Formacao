using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Interfaces.PalavraChave;
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
           [FromServices] ICasoDeUsoObterAreaPromotoraLista casoDeUsoObterAreaPromotoraLista)
        {
            return Ok(await casoDeUsoObterAreaPromotoraLista.Executar());
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
    }
}