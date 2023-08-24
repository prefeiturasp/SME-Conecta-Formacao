using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora
{
    public interface ICasoDeUsoObterAreaPromotoraPorId
    {
        Task<AreaPromotoraCompletoDTO> Executar(long id);
    }
}
