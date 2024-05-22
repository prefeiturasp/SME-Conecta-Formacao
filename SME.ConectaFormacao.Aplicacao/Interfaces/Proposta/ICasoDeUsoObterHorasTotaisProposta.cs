using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterHorasTotaisProposta
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}