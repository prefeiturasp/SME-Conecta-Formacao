using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCriterioValidacaoInscricao : IRepositorioBaseAuditavel<CriterioValidacaoInscricao>
    {
        Task<bool> ExisteCriterioValidacaoInscricaoOutros(long[] ids);
        Task<IEnumerable<CriterioValidacaoInscricao>> ObterIgnorandoExcluidos(bool exibirOutros);
    }
}
