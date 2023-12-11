using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.PalavraChave
{
    public interface ICasoDeUsoObterPalavraChave
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
