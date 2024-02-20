using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Formacao
{
    public interface ICasoDeUsoObterFormacaoHomologada
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
