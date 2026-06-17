using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafSuplementarRetificacao : IRepositorioBaseAuditavel<CodafSuplementarRetificacao>
    {
        Task<IEnumerable<CodafSuplementarRetificacao>> ObterPorCodafSuplementarIdAsync(long codafSuplementarId);
    }
}
