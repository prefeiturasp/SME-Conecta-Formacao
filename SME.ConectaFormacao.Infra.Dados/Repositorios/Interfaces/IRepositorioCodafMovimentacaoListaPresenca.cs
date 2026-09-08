using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafMovimentacaoListaPresenca
    {
        Task<long> InserirAsync(CodafMovimentacaoListaPresenca codafMovimentacaoListaPresenca);
        Task<CodafMovimentacaoListaPresenca?> ObterUltimaMovimentacaoPorListaPresencaIdAsync(long codafListaPresencaId);
        Task<CodafMovimentacaoListaPresenca?> ObterUltimaMovimentacaoPorListaPresencaStatusAsync(long codafListaPresencaId, StatusCodafListaPresenca status);
        Task<CodafMovimentacaoListaPresenca?> ObterPrimeiraMovimentacaoPorListaPresencaStatusAsync(long codafListaPresencaId, StatusCodafListaPresenca status);
    }
}