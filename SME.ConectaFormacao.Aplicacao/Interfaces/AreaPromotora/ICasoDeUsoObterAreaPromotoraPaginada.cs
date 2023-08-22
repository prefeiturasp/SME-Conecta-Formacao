using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora
{
    public interface ICasoDeUsoObterAreaPromotoraPaginada
    {
        Task<PaginacaoResultadoDTO<AreaPromotoraPaginadaDTO>> Executar(FiltrosAreaPromotoraDTO filtrosAreaPromotoraDTO);
    }
}
