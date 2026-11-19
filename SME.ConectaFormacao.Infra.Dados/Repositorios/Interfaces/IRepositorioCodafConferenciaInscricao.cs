using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafConferencias;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafConferenciaInscricao : IRepositorioBaseAuditavel<CodafConferenciaInscricao>
    {
        Task<ResultadoPaginado<ResultadoInscritoTurmaCodafConferenciaDto>> ObterInscritosPorTurmaAsync(long propostaTurmaId, int numeroPagina, int numeroRegistros, long? codafConferenciaId = null);
        Task InserirVariosAsync(IEnumerable<CodafConferenciaInscricao> inscritos);
        Task ExcluirPorCodafConferenciaIdAsync(long codafConferenciaId);
    }
}
