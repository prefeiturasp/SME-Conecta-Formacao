using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora
{
    public interface ICasoDeUsoObterAreaPromotoraListaRedeParceria
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
