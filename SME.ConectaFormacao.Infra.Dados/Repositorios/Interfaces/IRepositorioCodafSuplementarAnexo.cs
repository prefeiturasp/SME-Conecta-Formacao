using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafSuplementarAnexo : IRepositorioBaseAuditavel<CodafSuplementarAnexo>
    {
        Task<IEnumerable<CodafSuplementarAnexo>> ObterPorCodafSuplementarIdAsync(long codafSuplementarId);
    }
}
