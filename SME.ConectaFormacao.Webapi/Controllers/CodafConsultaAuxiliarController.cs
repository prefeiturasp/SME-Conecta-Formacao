using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Webapi.Controllers
{
    [Authorize("Bearer")]
    [Route("api/v1/CodafListaPresenca")]
    public class CodafConsultaAuxiliarController(
        ICasoDeUsoListarInscritosTurmaCodafListaPresenca casoDeUsoListarInscritosTurmaCodafListaPresenca,
        ICasoDeUsoTurmaPossuiCodafListaPresenca casoDeUsoTurmaPossuiCodafListaPresenca,
        ICasoDeUsoObterPropostaTurmaComCodaf casoDeUsoObterPropostaTurmaComCodaf) : BaseController
    {

        [HttpGet("inscritos-turma/{propostaTurmaId:long}")]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>>), 400)]
        [ProducesResponseType(typeof(Resultado<PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>>), 500)]
        public async Task<IActionResult> ObterInscritosPorTurma(long propostaTurmaId, [FromQuery] int numeroPagina = 1, [FromQuery] int numeroRegistros = 10)
        {
            var resultado = await casoDeUsoListarInscritosTurmaCodafListaPresenca.ExecutarAsync(propostaTurmaId, numeroPagina, numeroRegistros);
            return ProcessarResultado(resultado);
        }

        [HttpGet("turmas/{propostaTurmaId:long}/possui-lista")]
        [ProducesResponseType(typeof(Resultado<bool>), 200)]
        [ProducesResponseType(typeof(Resultado<bool>), 400)]
        public async Task<IActionResult> TurmaPossuiListaPresenca(long propostaTurmaId, [FromQuery] long listaPresencaId = 0)
        {
            var resultado = await casoDeUsoTurmaPossuiCodafListaPresenca.ExecutarAsync(propostaTurmaId, listaPresencaId);
            return ProcessarResultado(resultado);
        }

        [HttpGet("propostas/{propostaId:long}/turmas")]
        [ProducesResponseType(typeof(Resultado<IEnumerable<PropostaTurmaComCodafDto>>), 200)]
        [ProducesResponseType(typeof(Resultado<IEnumerable<PropostaTurmaComCodafDto>>), 400)]
        [ProducesResponseType(typeof(Resultado<IEnumerable<PropostaTurmaComCodafDto>>), 500)]
        public async Task<IActionResult> ObterTurmasComCodafPorProposta(long propostaId)
        {
            var resultado = await casoDeUsoObterPropostaTurmaComCodaf.ExecutarAsync(propostaId);
            return ProcessarResultado(resultado);
        }
    }
}
