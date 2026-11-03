using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioPropostaGrupoPeriodoInscricao : IRepositorioBaseAuditavel<PropostaGrupoPeriodoInscricao>
    {
        Task<IEnumerable<PropostaGrupoPeriodoInscricao>> ObterPorPropostaIdAsync(long propostaId);
    }
}
