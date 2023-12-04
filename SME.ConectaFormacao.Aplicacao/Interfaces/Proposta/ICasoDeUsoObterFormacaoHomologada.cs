using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterFormacaoHomologada
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
