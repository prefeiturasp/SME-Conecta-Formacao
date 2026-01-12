using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafAnexo : IRepositorioBaseAuditavel<CodafAnexo>
    {
        Task<IEnumerable<CodafAnexo>> ObterPorCodafIdAsync(long codafListaPresencaId);
    }
}
