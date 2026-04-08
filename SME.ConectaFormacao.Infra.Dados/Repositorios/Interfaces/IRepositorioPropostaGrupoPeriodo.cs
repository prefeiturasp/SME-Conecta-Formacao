using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioPropostaGrupoPeriodo : IRepositorioBaseAuditavel<PropostaGrupoPeriodo>
    {
        Task<IEnumerable<PropostaGrupoPeriodo>> ObterPorPropostaIdAsync(long propostaId);
    }
}
