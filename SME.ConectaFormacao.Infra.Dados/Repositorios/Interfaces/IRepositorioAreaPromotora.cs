using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using System.Data;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioAreaPromotora : IRepositorioBaseAuditavel<AreaPromotora>
    {
        Task<bool> Atualizar(IDbTransaction transacao, AreaPromotora areaPromotora);
        Task<long> Inserir(IDbTransaction transacao, AreaPromotora areaPromotora);
        Task<bool> Remover(IDbTransaction transacao, AreaPromotora areaPromotora);
        Task<IEnumerable<AreaPromotora>> ObterDadosPaginados(string nome, short? tipo, int numeroPagina, int numeroRegistros);
        Task<IEnumerable<AreaPromotoraTelefone>> ObterTelefonesPorId(long id);
        Task<int> ObterTotalRegistrosPorFiltros(string nome, short? tipo);
        Task InserirTelefones(IDbTransaction transacao, long id, IEnumerable<AreaPromotoraTelefone> telefones);
        Task RemoverTelefones(IDbTransaction transacao, long id, IEnumerable<AreaPromotoraTelefone> telefones);
        Task<AreaPromotora> ObterPorGrupoId(Guid grupoId);
        Task<bool> ExistePorGrupoId(Guid grupoId, long ignorarAreaPromotoraId);
        Task<IEnumerable<AreaPromotora>> ObterLista();
        Task<AreaPromotora> ObterAreaPromotoraPorIdComDre(long areaPromotoraId);
        Task<bool> ExistePorGrupoIdEDreId(long dreId, Guid grupoId, long ignorarAreaPromotoraId);
    }
}
