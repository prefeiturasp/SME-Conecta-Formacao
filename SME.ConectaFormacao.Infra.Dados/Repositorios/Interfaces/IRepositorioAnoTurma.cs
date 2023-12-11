using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioAnoTurma : IRepositorioBaseAuditavel<AnoTurma>
    {
        Task<IEnumerable<AnoTurma>> ObterAnosPorModalidadeAnoLetivo(Modalidade[] modalidade, int anoLetivo, bool exibirTodos);
        Task<IEnumerable<AnoTurma>> ObterPorAnoLetivo(int anoLetivo);
    }
}