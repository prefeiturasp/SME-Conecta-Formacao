using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterModalidades
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
