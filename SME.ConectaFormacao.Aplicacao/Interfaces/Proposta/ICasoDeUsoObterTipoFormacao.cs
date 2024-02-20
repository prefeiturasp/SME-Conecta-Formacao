using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterTipoFormacao
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
