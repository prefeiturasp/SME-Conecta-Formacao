using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioImportacaoArquivoRegistro : IRepositorioBaseAuditavel<ImportacaoArquivoRegistro>
    {
        Task<RegistrosPaginados<ImportacaoArquivoRegistro>> ObterRegistrosComErro(int quantidadeRegistroIgnorados, int numeroRegistros, long arquivoId);
        Task<RegistrosPaginados<ImportacaoArquivoRegistro>> ObterRegistrosValidados(int quantidadeRegistroIgnorados, int numeroRegistros, long arquivoId);
    }
}
