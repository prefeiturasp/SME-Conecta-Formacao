using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora
{
    public interface ICasoDeUsoAlterarAreaPromotora
    {
        Task<bool> Executar(long id, AreaPromotoraDTO areaPromotoraDTO);
    }
}
