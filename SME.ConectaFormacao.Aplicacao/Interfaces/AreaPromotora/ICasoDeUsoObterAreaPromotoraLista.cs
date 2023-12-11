using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora
{
    public interface ICasoDeUsoObterAreaPromotoraLista
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
