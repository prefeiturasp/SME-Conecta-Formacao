using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Webapi.Controllers.Filtros;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    public class InscricaoController : BaseController
    {
        [HttpGet("dados-inscricao")]
        [ProducesResponseType(typeof(DadosInscricaoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterDadosUsuario(
            [FromServices] ICasoDeUsoObterDadosInscricao casoDeUsoObterDadosInscricao)
        {
            return Ok(await casoDeUsoObterDadosInscricao.Executar());
        }

        [HttpGet("turmas/{propostaId}")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterTurmas(
            [FromServices] ICasoDeUsoObterTurmasInscricao casoDeUsoObterTurmasInscricao,
            [FromRoute] long propostaId,
            [FromQuery] string? codigoDre = null)
        {
            return Ok(await casoDeUsoObterTurmasInscricao.Executar(propostaId, codigoDre));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginacaoResultadoDTO<InscricaoPaginadaDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> ObterInscricoesPaginada(
            [FromServices] ICasoDeUsoObterInscricaoPaginada casoDeUsoObterInscricaoPaginada)
        {
            return Ok(await casoDeUsoObterInscricaoPaginada.Executar());
        }

        [HttpPost]
        [ProducesResponseType(typeof(RetornoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> SalvarInscricao(
            [FromServices] ICasoDeUsoSalvarInscricao casoDeUsoSalvarInscricao,
            [FromBody] InscricaoDTO inscricaoDTO)
        {
            return Ok(await casoDeUsoSalvarInscricao.Executar(inscricaoDTO));
        }

        [HttpPost("manual")]
        [ProducesResponseType(typeof(RetornoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> SalvarInscricaoManual(
            [FromServices] ICasoDeUsoSalvarInscricaoManual casoDeUsoSalvarInscricaoManual,
            [FromBody] InscricaoManualDTO inscricaoDTO)
        {
            return Ok(await casoDeUsoSalvarInscricaoManual.Executar(inscricaoDTO));
        }

        [HttpPut("{id}/cancelar")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> CancelarInscricao(
            [FromServices] ICasoDeUsoCancelarInscricao casoDeUsoSalvarInscricao,
            [FromRoute] long id)
        {
            return Ok(await casoDeUsoSalvarInscricao.Executar(id));
        }

        [HttpPut("confirmar")]
        [ProducesResponseType(typeof(RetornoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Inscricao_I, Permissao.Inscricao_A, Permissao.Inscricao_E, Policy = "Bearer")]
        public async Task<IActionResult> ConfirmarInscricoes(
            [FromServices] ICasoDeUsoConfirmarInscricoes casoDeUso,
            [FromQuery] long[] ids)
        {
            return Ok(await casoDeUso.Executar(ids));
        }

        [HttpPut("em-espera")]
        [ProducesResponseType(typeof(RetornoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Inscricao_I, Permissao.Inscricao_A, Permissao.Inscricao_E, Policy = "Bearer")]
        public async Task<IActionResult> EmEsperaInscricoes(
            [FromServices] ICasoDeUsoEmEsperaInscricoes casoDeUso,
            [FromQuery] long[] ids)
        {
            return Ok(await casoDeUso.Executar(ids));
        }

        [HttpPut("cancelar")]
        [ProducesResponseType(typeof(RetornoDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Inscricao_I, Permissao.Inscricao_A, Permissao.Inscricao_E, Policy = "Bearer")]
        public async Task<IActionResult> CancelarInscricoes(
            [FromServices] ICasoDeUsoCancelarInscricoes casoDeUso,
            [FromQuery] long[] ids, 
            [FromBody] InscricaoMotivoCancelamentoDTO inscricaoMotivoCancelamentoDTO)
        {
            return Ok(await casoDeUso.Executar(ids, inscricaoMotivoCancelamentoDTO.Motivo));
        }

        [HttpGet("{propostaId}")]
        [ProducesResponseType(typeof(PaginacaoResultadoDTO<DadosListagemInscricaoDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Inscricao_I, Permissao.Inscricao_A, Permissao.Inscricao_E, Policy = "Bearer")]
        public async Task<IActionResult> ObterInscricaoPorIdPaginado([FromRoute] long propostaId, [FromQuery] FiltroListagemInscricaoDTO filtroListagemInscricaoDTO,
        [FromServices] ICasoDeUsoObterInscricaoPorId casoDeUsoObterInscricaoPorId)
        {
            return Ok(await casoDeUsoObterInscricaoPorId.Executar(propostaId, filtroListagemInscricaoDTO));
        }

        [HttpGet("formacao-turmas")]
        [ProducesResponseType(typeof(PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Inscricao_I, Permissao.Inscricao_A, Permissao.Inscricao_E, Policy = "Bearer")]
        public async Task<IActionResult> ObterFormacaoComTurmaPorFiltros([FromQuery] FiltroListagemInscricaoComTurmaDTO filtro, [FromServices] ICasoDeUsoObterDadosPaginadosComFiltros useCase)
        {
            return Ok(await useCase.Executar(filtro));
        }

        [HttpGet("tipos")]
        [ProducesResponseType(typeof(IEnumerable<RetornoListagemDTO>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Inscricao_I, Permissao.Inscricao_A, Permissao.Inscricao_E, Policy = "Bearer")]
        public async Task<IActionResult> ObterInscricaoTipo(
            [FromServices] ICasoDeUsoObterInscricaoTipo casoDeUsoObterInscricaoTipo)
        {
            return Ok(await casoDeUsoObterInscricaoTipo.Executar());
        }

        [HttpGet("cursista")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Inscricao_I, Permissao.Inscricao_A, Permissao.Inscricao_E, Policy = "Bearer")]
        public async Task<IActionResult> ObterCursistaInscricao(
            [FromServices] ICasoDeUsoObterNomeCpfCursistaInscricao casoDeUsoObterCpfCursistaInscricao,
            [FromQuery] string? registroFuncional,
            [FromQuery] string? cpf)
        {
            return Ok(await casoDeUsoObterCpfCursistaInscricao.Executar(registroFuncional, cpf));
        }

        [HttpPut("{id}/alterar-vinculo")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        public async Task<IActionResult> AlterarVinculo(
            [FromServices] ICasoDeUsoAlterarVinculoInscricao casoDeUsoAlterarVinculoInscricao,
            [FromRoute] long id,
            [FromBody] AlterarCargoFuncaoVinculoIncricaoDTO alterarCargoFuncaoVinculoIncricao)
        {
            return Ok(await casoDeUsoAlterarVinculoInscricao.Executar(id, alterarCargoFuncaoVinculoIncricao));
        }

        [HttpGet("{propostaId}/abertas")]
        [ProducesResponseType(typeof(PodeInscreverMensagemDTO), 200)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 400)]
        [ProducesResponseType(typeof(RetornoBaseDTO), 500)]
        [Permissao(Permissao.Inscricao_I, Permissao.Inscricao_A, Permissao.Inscricao_E, Policy = "Bearer")]
        public async Task<IActionResult> InscricoesEstaoAbertas([FromRoute] long propostaId,
            [FromServices] ICasoDeUsoObterInformacoesInscricoesEstaoAbertasPorId casoDeUsoObterInformacoesInscricoesEstaoAbertasPorId)
        {
            return Ok(await casoDeUsoObterInformacoesInscricoesEstaoAbertasPorId.Executar(propostaId));
        }
    }
}
