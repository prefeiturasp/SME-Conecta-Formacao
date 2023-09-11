using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterSituacoesProposta
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
