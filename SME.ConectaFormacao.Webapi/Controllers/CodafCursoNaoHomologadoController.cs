using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Webapi.Filtros;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    [PadronizarRetornoFiltro]
    public class CodafCursoNaoHomologadoController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(Resultado<CodafCursoNaoHomologadoDetalhadoDto>), 201)]
        [ProducesResponseType(typeof(Resultado<CodafCursoNaoHomologadoDetalhadoDto>), 400)]
        [ProducesResponseType(typeof(Resultado<CodafCursoNaoHomologadoDetalhadoDto>), 404)]
        [ProducesResponseType(typeof(Resultado<CodafCursoNaoHomologadoDetalhadoDto>), 422)]
        public async Task<IActionResult> Cadastrar(
            [FromBody] CodafCursoNaoHomologadoCadastroDto codafCursoNaoHomologadoCadastroDto,
            [FromServices] ICasoDeUsoCriarCodafCursoNaoHomologado casoDeUsoCriarCodafCursoNaoHomologado)
        {
            var resultado = await casoDeUsoCriarCodafCursoNaoHomologado.ExecutarAsync(codafCursoNaoHomologadoCadastroDto);
            return ProcessarCriado(resultado);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(Resultado), 204)]
        [ProducesResponseType(typeof(Resultado), 400)]
        [ProducesResponseType(typeof(Resultado), 404)]
        [ProducesResponseType(typeof(Resultado), 422)]
        public async Task<IActionResult> Atualizar(
            int id, [FromBody] CodafCursoNaoHomologadoCadastroDto codafCursoNaoHomologadoCadastroDto,
            [FromServices] ICasoDeUsoAtualizarCodafCursoNaoHomologado casoDeUsoAtualizar)
        {
            var resultado = await casoDeUsoAtualizar.ExecutarAsync(codafCursoNaoHomologadoCadastroDto, id);
            return ProcessarResultado(resultado);
        }

        [HttpDelete("{codafCursoNaoHomologadoId:long}")]
        public async Task<IActionResult> Excluir(long codafCursoNaoHomologadoId,
            [FromServices] ICasoDeUsoExcluirCodafCursoNaoHomologado casoDeUsoExcluirCodafCursoNaoHomologado)
        {
            var resultado = await casoDeUsoExcluirCodafCursoNaoHomologado.ExecutarAsync(codafCursoNaoHomologadoId);
            return ProcessarResultado(resultado);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafCursoNaoHomologadoResumoDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafCursoNaoHomologadoResumoDto>>), 400)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafCursoNaoHomologadoResumoDto>>), 500)]
        public async Task<IActionResult> ObterListaPaginada(
            [FromQuery] FiltroCodafCursoNaoHomologadoDto filtro,
            [FromServices] ICasoDeUsoListarCodafCursoNaoHomologado casoDeUsoListarCodafCursoNaoHomologado)
        {
            var resultado = await casoDeUsoListarCodafCursoNaoHomologado.ExecutarAsync(filtro);
            return ProcessarResultado(resultado);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(Resultado<CodafCursoNaoHomologadoDetalhadoDto>), 200)]
        [ProducesResponseType(typeof(Resultado<CodafCursoNaoHomologadoDetalhadoDto>), 404)]
        public async Task<IActionResult> ObterPorId(long id,
            [FromServices] ICasoDeUsoObterCodafCursoNaoHomologadoPorId casoDeUsoObterCodafCursoNaoHomologadoPorId)
        {
            var resultado = await casoDeUsoObterCodafCursoNaoHomologadoPorId.ExecutarAsync(id);
            return ProcessarResultado(resultado);
        }

        [HttpGet("inscritos-turma/{propostaTurmaId:long}")]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafCursoNaoHomologadoInscritoTurmaDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafCursoNaoHomologadoInscritoTurmaDto>>), 400)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafCursoNaoHomologadoInscritoTurmaDto>>), 500)]
        public async Task<IActionResult> ObterInscritosPorTurma(long propostaTurmaId, [FromServices] ICasoDeUsoListarInscritosTurmaCodafCursoNaoHomologado casoDeUso,
            [FromQuery] int numeroPagina = 1, [FromQuery] int numeroRegistros = 10)
        {
            var resultado = await casoDeUso.ExecutarAsync(propostaTurmaId, numeroPagina, numeroRegistros);
            return ProcessarResultado(resultado);
        }
    }
}
