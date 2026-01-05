using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafInscritosListaPresenca : IRepositorioBaseAuditavel<CodafInscricaoListaPresenca>
    {
        Task<ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>> ObterInscritosPorTurmaAsync(long propostaTurmaId, int numeroPagina, int numeroRegistros);
        Task InserirVariosAsync(IEnumerable<CodafInscricaoListaPresenca> inscritosListaPresenca);
        Task ExcluirPorListaPresencaIdAsync(long codafListaPresencaId);
    }
}