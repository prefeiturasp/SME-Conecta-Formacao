using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioComponenteCurricular : IRepositorioBaseAuditavel<ComponenteCurricular>
    {
        Task<IEnumerable<ComponenteCurricular>> ObterComponentesCurricularesPorAnoTurma(long[] anoTurmaId, bool exibirOpcaoTodos);
        Task<IEnumerable<ComponenteCurricular>> ObterPorAnoLetivo(int anoLetivo);
    }
}