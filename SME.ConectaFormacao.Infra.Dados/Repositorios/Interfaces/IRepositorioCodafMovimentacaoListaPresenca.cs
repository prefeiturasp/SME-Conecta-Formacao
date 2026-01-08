using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafMovimentacaoListaPresenca
    {
        Task<long> InserirAsync(CodafMovimentacaoListaPresenca codafMovimentacaoListaPresenca);
        Task<CodafMovimentacaoListaPresenca?> ObterUltimaMovimentacaoPorListaPresencaIdAsync(long codafListaPresencaId);
    }
}