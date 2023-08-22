using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora
{
    public interface ICasoDeUsoObterTiposAreaPromotora
    {
        Task<IEnumerable<AreaPromotoraTipoDTO>> Executar();
    }
}
