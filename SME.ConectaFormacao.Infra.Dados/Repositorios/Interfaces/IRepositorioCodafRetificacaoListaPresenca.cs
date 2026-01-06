using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafRetificacaoListaPresenca : IRepositorioBaseAuditavel<CodafRetificacaoListaPresenca>
    {
        Task<IEnumerable<CodafRetificacaoListaPresenca>> ObterPorListaPresencaIdAsync(long codafListaPresencaId);
    }
}
