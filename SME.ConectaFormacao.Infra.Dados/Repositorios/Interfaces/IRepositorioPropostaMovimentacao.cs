using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioPropostaMovimentacao : IRepositorioBaseAuditavel<PropostaMovimentacao>
    {
        Task<PropostaMovimentacao> ObterUltimoParecerPropostaId(long propostaId);
        Task<string> ObterUltimaJustificativaDevolucao(long propostaId);
    }
}
