using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora
{
    public interface ICasoDeUsoObterTiposAreaPromotora
    {
        Task<IEnumerable<AreaPromotoraTipoDTO>> Executar();
    }
}
