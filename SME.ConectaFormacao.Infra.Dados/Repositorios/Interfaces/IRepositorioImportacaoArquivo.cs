using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricao;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioImportacaoArquivo : IRepositorioBaseAuditavel<ImportacaoArquivo>
    {
        Task<ArquivosInscricaoPaginadoDto> ObterArquivosInscricaoImportacao(int quantidadeRegistroIgnorados, int numeroRegistros, long propostaId);
    }
}
