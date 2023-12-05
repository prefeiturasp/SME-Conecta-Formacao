using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioComponenteCurricular : IRepositorioBaseAuditavel<ComponenteCurricular>
    {
        Task<IEnumerable<ComponenteCurricular>> ObterComponentesCurricularesPorModalidadeAnoLetivoAno(Modalidade modalidade, int anoLetivo, int ano);
    }
}