using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterTipoEncontro
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
