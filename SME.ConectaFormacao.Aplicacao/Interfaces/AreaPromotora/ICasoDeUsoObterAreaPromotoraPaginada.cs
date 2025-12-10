using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora
{
    public interface ICasoDeUsoObterAreaPromotoraPaginada
    {
        Task<PaginacaoResultadoDto<AreaPromotoraPaginadaDTO>> Executar(AreaPromotoraFiltrosDTO filtrosAreaPromotoraDTO);
    }
}
