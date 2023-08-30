using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora
{
    public interface ICasoDeUsoInserirAreaPromotora
    {
        Task<long> Executar(AreaPromotoraDTO areaPromotoraDTO);
    }
}
