using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoObterTurmasInscricao
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar(long propostaId, string? codigoDre = null);
    }
}
