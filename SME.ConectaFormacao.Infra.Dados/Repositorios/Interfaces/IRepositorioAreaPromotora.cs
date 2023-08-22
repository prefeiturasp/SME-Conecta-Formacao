using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using System.Data;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioAreaPromotora : IRepositorioBaseAuditavel<AreaPromotora>
    {
        Task<long> Inserir(IDbTransaction transacao, AreaPromotora areaPromotora);
        Task InserirTelefones(IDbTransaction transacao, long id, IEnumerable<AreaPromotoraTelefone> telefones);
        Task<IEnumerable<AreaPromotora>> ObterDadosPaginados(string nome, short? tipo, int numeroPagina, int numeroRegistros);
        Task<int> ObterTotalRegistrosPorFiltros(string nome, short? tipo);
    }
}
