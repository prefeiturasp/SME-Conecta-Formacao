using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafConferencias;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafConferencia : IRepositorioBaseAuditavel<CodafConferencia>
    {
        Task<ResultadoPaginado<ListagemResultadoCodafConferenciaDto>> ObterListagemPorFiltroAsync(FiltroListagemResultadoCodafConferenciaDto filtro);
        Task<CodafConferencia?> ObterPorIdDetalhadoAsync(long id);
        Task ExcluirAsync(long id);
        Task<bool> TurmaJaTemCodafAsync(long propostaTurmaId, long? codafConferenciaId = null);
    }
}
