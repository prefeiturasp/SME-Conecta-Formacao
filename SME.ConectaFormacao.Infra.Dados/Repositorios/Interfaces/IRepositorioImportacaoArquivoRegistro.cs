using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioImportacaoArquivoRegistro : IRepositorioBaseAuditavel<ImportacaoArquivoRegistro>
    {
        Task<IEnumerable<ImportacaoArquivoRegistro>> ObterRegistrosImportacaoArquivoInscricaoCursistasPaginados(long importacaoArquivoId, int numeroRegistros, int quantidadeRegistroIgnorados);
    }
}
