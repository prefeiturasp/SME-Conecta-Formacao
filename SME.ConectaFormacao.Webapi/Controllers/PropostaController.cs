using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class PropostaController : BaseController
    {
        [HttpGet("informacoes-cadastrante")]
        [ProducesResponseType(typeof(PropostaInformacoesCadastranteDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterInformacoesCadastrante(
            [FromServices] ICasoDeUsoObterInformacoesCadastrante casoDeUsoObterInformacoesCadastrante)
        {
            return Ok(await casoDeUsoObterInformacoesCadastrante.Executar());
        }

        [HttpGet("roteiro")]
        [ProducesResponseType(typeof(RoteiroPropostaFormativaDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterRoteiroPropostaFormativa(
            [FromServices] ICasoDeUsoObterRoteiroPropostaFormativa casoDeUsoObterRoteiroPropostaFormativa)
        {
            return Ok(await casoDeUsoObterRoteiroPropostaFormativa.Executar());
        }

        [HttpGet("criterio-validacao-inscricao")]
        [ProducesResponseType(typeof(IEnumerable<CriterioValidacaoInscricaoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterCriterioValidacaoInscricao(
            [FromServices] ICasoDeUsoObterCriterioValidacaoInscricao casoDeUsoObterCriterioValidacaoInscricao,
            [FromQuery] bool exibirOpcaoOutros = false)
        {
            return Ok(await casoDeUsoObterCriterioValidacaoInscricao.Executar(exibirOpcaoOutros));
        }

        [HttpGet("tipo-formacao")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterTipoFormacao(
            [FromServices] ICasoDeUsoObterTipoFormacao casoDeUsoObterTipoFormacao)
        {
            return Ok(await casoDeUsoObterTipoFormacao.Executar());
        }

        [HttpGet("tipo-inscricao")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterTipoInscricao(
            [FromServices] ICasoDeUsoObterTipoInscricao casoDeUsoObterTipoInscricao)
        {
            return Ok(await casoDeUsoObterTipoInscricao.Executar());
        }

        [HttpGet("modalidades/tipo-formacao/{tipoFormacao}")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterModalidades(
            [FromServices] ICasoDeUsoObterModalidades casoDeUsoObterModalidades,
            [FromRoute] TipoFormacao tipoFormacao)
        {
            return Ok(await casoDeUsoObterModalidades.Executar(tipoFormacao));
        }

        [HttpGet("situacao")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterSituacoes(
            [FromServices] ICasoDeUsoObterSituacoesProposta casoDeUsoObterSituacoesProposta)
        {
            return Ok(await casoDeUsoObterSituacoesProposta.Executar());
        }

        [HttpGet("tipo-encontro")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterTipoEncontro(
            [FromServices] ICasoDeUsoObterTipoEncontro casoDeUsoObterTipoEncontro)
        {
            return Ok(await casoDeUsoObterTipoEncontro.Executar());
        }

        [HttpGet("{id}/turma")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterTurmas(
            [FromServices] ICasoDeUsoObterTurmasProposta casoDeUsoObterTurmasProposta,
            [FromRoute] long id)
        {
            return Ok(await casoDeUsoObterTurmasProposta.Executar(id));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PropostaCompletoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterPropostaPorId(
            [FromServices] ICasoDeUsoObterPropostaPorId casoDeUsoObterPropostaPorId,
            [FromRoute] long id)
        {
            return Ok(await casoDeUsoObterPropostaPorId.Executar(id));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginacaoResultadoDTO<PropostaPaginadaDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterPropostaPaginada(
            [FromServices] ICasoDeUsoObterPropostaPaginacao casoDeUsoObterPropostaPaginacao,
            [FromQuery] PropostaFiltrosDTO propostaFiltrosDTO)
        {
            return Ok(await casoDeUsoObterPropostaPaginacao.Executar(propostaFiltrosDTO));
        }

        [HttpPost]
        [ProducesResponseType(typeof(long), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> InserirProposta(
            [FromServices] ICasoDeUsoInserirProposta casoDeUsoInserirProposta,
            [FromBody] PropostaDTO propostaDTO)
        {
            return Ok(await casoDeUsoInserirProposta.Executar(propostaDTO));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(long), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> AlterarProposta(
           [FromServices] ICasoDeUsoAlterarProposta casoDeUsoAlterarProposta,
           [FromRoute] long id,
           [FromBody] PropostaDTO propostaDTO)
        {
            return Ok(await casoDeUsoAlterarProposta.Executar(id, propostaDTO));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> RemoverProposta(
          [FromServices] ICasoDeUsoRemoverProposta casoDeUsoRemoverProposta,
          [FromRoute] long id)
        {
            return Ok(await casoDeUsoRemoverProposta.Executar(id));
        }
    }
}
