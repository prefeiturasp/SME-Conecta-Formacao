using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioArquivo : IRepositorioBaseAuditavel<Arquivo>
    {
        Task<Arquivo> ObterPorCodigo(Guid codigo);
        Task<IEnumerable<Arquivo>> ObterPorCodigos(Guid[] codigos);
        Task<IEnumerable<Arquivo>> ObterPorIds(long[] ids);
        Task<bool> ExcluirArquivoPorCodigo(Guid codigoArquivo);
        Task<bool> ExcluirArquivoPorId(long id);
        Task<long> ObterIdPorCodigo(Guid arquivoCodigo);
        Task<bool> ExcluirArquivosPorIds(long[] ids);
    }
}
