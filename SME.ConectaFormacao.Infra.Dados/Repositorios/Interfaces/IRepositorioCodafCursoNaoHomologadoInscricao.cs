using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafCursoNaoHomologadoInscricao : IRepositorioBaseAuditavel<CodafCursoNaoHomologadoInscricao>
    {
        Task<ResultadoPaginado<ResultadoInscritoTurmaCodafCursoNaoHomologadoDto>> ObterInscritosPorTurmaAsync(long propostaTurmaId, int numeroPagina, int numeroRegistros);
        Task InserirVariosAsync(IEnumerable<CodafCursoNaoHomologadoInscricao> inscritosCursoNaoHomologado);
        Task ExcluirPorCursoNaoHomologadoIdAsync(long codafCursoNaoHomologadoId);
    }
}