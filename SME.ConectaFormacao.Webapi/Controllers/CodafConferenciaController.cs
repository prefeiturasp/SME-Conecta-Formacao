using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Webapi.Filtros;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    [PadronizarRetornoFiltro]
    [Route("api/v1/codaf-conferencia")]
    public class CodafConferenciaController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(Resultado<CodafConferenciaDetalhadoDto>), 201)]
        [ProducesResponseType(typeof(Resultado<CodafConferenciaDetalhadoDto>), 400)]
        [ProducesResponseType(typeof(Resultado<CodafConferenciaDetalhadoDto>), 404)]
        [ProducesResponseType(typeof(Resultado<CodafConferenciaDetalhadoDto>), 422)]
        public async Task<IActionResult> Cadastrar(
            [FromBody] CodafConferenciaCadastroDto dto,
            [FromServices] ICasoDeUsoCriarCodafConferencia casoDeUsoCriar)
        {
            var resultado = await casoDeUsoCriar.ExecutarAsync(dto);
            return ProcessarCriado(resultado);
        }

        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(Resultado), 204)]
        [ProducesResponseType(typeof(Resultado), 400)]
        [ProducesResponseType(typeof(Resultado), 404)]
        [ProducesResponseType(typeof(Resultado), 422)]
        public async Task<IActionResult> Atualizar(
            long id,
            [FromBody] CodafConferenciaCadastroDto dto,
            [FromServices] ICasoDeUsoAtualizarCodafConferencia casoDeUsoAtualizar)
        {
            var resultado = await casoDeUsoAtualizar.ExecutarAsync(dto, id);
            return ProcessarResultado(resultado);
        }

        [HttpDelete("{id:long}")]
        [ProducesResponseType(typeof(Resultado), 204)]
        [ProducesResponseType(typeof(Resultado), 400)]
        [ProducesResponseType(typeof(Resultado), 404)]
        [ProducesResponseType(typeof(Resultado), 422)]
        public async Task<IActionResult> Excluir(
            long id,
            [FromServices] ICasoDeUsoExcluirCodafConferencia casoDeUsoExcluir)
        {
            var resultado = await casoDeUsoExcluir.ExecutarAsync(id);
            return ProcessarResultado(resultado);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafConferenciaResumoDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafConferenciaResumoDto>>), 400)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafConferenciaResumoDto>>), 500)]
        public async Task<IActionResult> ObterListaPaginada(
            [FromQuery] FiltroCodafConferenciaDto filtro,
            [FromServices] ICasoDeUsoListarCodafConferencia casoDeUsoListar)
        {
            var resultado = await casoDeUsoListar.ExecutarAsync(filtro);
            return ProcessarResultado(resultado);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(Resultado<CodafConferenciaDetalhadoDto>), 200)]
        [ProducesResponseType(typeof(Resultado<CodafConferenciaDetalhadoDto>), 404)]
        public async Task<IActionResult> ObterPorId(
            long id,
            [FromServices] ICasoDeUsoObterCodafConferenciaPorId casoDeUsoObterPorId)
        {
            var resultado = await casoDeUsoObterPorId.ExecutarAsync(id);
            return ProcessarResultado(resultado);
        }

        [HttpGet("inscritos-turma/{propostaTurmaId:long}")]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafConferenciaInscritoTurmaDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafConferenciaInscritoTurmaDto>>), 400)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafConferenciaInscritoTurmaDto>>), 500)]
        public async Task<IActionResult> ObterInscritosPorTurma(
            long propostaTurmaId,
            [FromServices] ICasoDeUsoListarInscritosTurmaCodafConferencia casoDeUso,
            [FromQuery] int numeroPagina = 1,
            [FromQuery] int numeroRegistros = 10,
            [FromQuery] long? codafConferenciaId = null)
        {
            var resultado = await casoDeUso.ExecutarAsync(propostaTurmaId, numeroPagina, numeroRegistros, codafConferenciaId);
            return ProcessarResultado(resultado);
        }

        [HttpGet("propostas/{propostaId:long}/detalhes-com-turmas")]
        [ProducesResponseType(typeof(Resultado<PropostaComTurmasDto?>), 200)]
        [ProducesResponseType(typeof(Resultado<PropostaComTurmasDto?>), 400)]
        public async Task<IActionResult> ObterDetalhesPropostaComTurmasPorId(
            long propostaId,
            [FromServices] ICasoDeUsoObterDetalhesPropostaConferenciaComTurmasPorId casoDeUso)
        {
            var resultado = await casoDeUso.ExecutarAsync(propostaId);
            return ProcessarResultado(resultado);
        }

        [HttpGet("turmas/{propostaTurmaId:long}/possui-codaf")]
        [ProducesResponseType(typeof(Resultado<bool>), 200)]
        [ProducesResponseType(typeof(Resultado<bool>), 400)]
        public async Task<IActionResult> TurmaPossuiCodaf(
            long propostaTurmaId,
            [FromServices] ICasoDeUsoTurmaPossuiCodafConferencia casoDeUso,
            [FromQuery] long? codafConferenciaId = null)
        {
            var resultado = await casoDeUso.ExecutarAsync(propostaTurmaId, codafConferenciaId);
            return ProcessarResultado(resultado);
        }

        [HttpPost("anexos/temporarios")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Resultado<CodafAnexoTemporarioDto>), 200)]
        [ProducesResponseType(typeof(Resultado<CodafAnexoTemporarioDto>), 400)]
        public async Task<IActionResult> UploadAnexoTemporario(
            [FromForm] IFormFile arquivo,
            [FromServices] ICasoDeUsoUploadAnexoTemporarioCodafConferencia casoDeUsoUpload)
        {
            var resultado = await casoDeUsoUpload.ExecutarAsync(arquivo);
            return ProcessarResultado(resultado);
        }
    }
}
