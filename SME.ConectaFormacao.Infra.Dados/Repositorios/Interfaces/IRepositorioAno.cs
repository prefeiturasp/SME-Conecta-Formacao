using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioAno : IRepositorioBaseAuditavel<Ano>
    {
        Task<IEnumerable<Ano>> ObterAnosPorModalidadeAnoLetivo(Modalidade modalidade, int anoLetivo);
    }
}