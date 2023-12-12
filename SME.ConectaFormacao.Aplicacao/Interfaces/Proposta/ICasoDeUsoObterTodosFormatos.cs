using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterTodosFormatos
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
