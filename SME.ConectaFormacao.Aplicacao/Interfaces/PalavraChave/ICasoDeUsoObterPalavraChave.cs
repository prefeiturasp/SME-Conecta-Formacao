using SME.ConectaFormacao.Aplicacao.Dtos.PalavraChave;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.PalavraChave
{
    public interface ICasoDeUsoObterPalavraChave
    {
        Task<IEnumerable<PalavraChaveDTO>> Executar();
    }
}
