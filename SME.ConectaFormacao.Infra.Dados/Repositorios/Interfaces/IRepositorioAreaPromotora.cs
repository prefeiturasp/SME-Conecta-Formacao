using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioAreaPromotora : IRepositorioBaseAuditavel<AreaPromotora>
    {
        Task<IEnumerable<AreaPromotora>> ObterDadosPaginados(string nome, short? tipo, int numeroPagina, int numeroRegistros);
        Task<int> ObterTotalRegistrosPorFiltros(string nome, short? tipo);
    }
}
