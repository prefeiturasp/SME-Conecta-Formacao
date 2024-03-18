using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioImportacaoArquivoRegistro : IRepositorioBaseAuditavel<ImportacaoArquivoRegistro>
    {
        Task<RegistrosPaginados<ImportacaoArquivoRegistro>> ObterRegistrosComErro(int quantidadeRegistroIgnorados, int numeroRegistros, long importacaoArquivoId);
        Task<RegistrosPaginados<ImportacaoArquivoRegistro>> ObterRegistroPorSituacao(int quantidadeRegistroIgnorados, int numeroRegistros, long arquivoId, SituacaoImportacaoArquivoRegistro situacao);
        Task<bool> TodosRegistroForamProcessadosDoArquivo(long importacaoArquivoId);
    }
}
