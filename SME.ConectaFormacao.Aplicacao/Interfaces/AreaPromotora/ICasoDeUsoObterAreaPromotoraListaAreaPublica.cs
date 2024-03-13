using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora
{
    public interface ICasoDeUsoObterAreaPromotoraListaAreaPublica
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
