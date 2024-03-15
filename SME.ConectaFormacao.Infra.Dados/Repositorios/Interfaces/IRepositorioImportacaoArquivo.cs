using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioImportacaoArquivo : IRepositorioBaseAuditavel<ImportacaoArquivo>
    {
        Task<RegistrosPaginados<ArquivosImportadosTotalRegistro>> ObterArquivosInscricaoImportacao(int quantidadeRegistroIgnorados, int numeroRegistros, long propostaId);
        Task<IEnumerable<ImportacaoArquivo>> ObterImportacaoArquivosValidados(long propostaId);
    }
}
