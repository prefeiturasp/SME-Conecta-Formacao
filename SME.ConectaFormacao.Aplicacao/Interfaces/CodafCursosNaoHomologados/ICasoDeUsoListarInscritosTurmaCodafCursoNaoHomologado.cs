using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados
{
    public interface ICasoDeUsoListarInscritosTurmaCodafCursoNaoHomologado
    {
        Task<Resultado<PaginacaoResultadoDto<CodafCursoNaoHomologadoInscritoTurmaDto>>> ExecutarAsync(long propostaTurmaId, int numeroPagina = 1, int numeroRegistros = 10);
    }
}