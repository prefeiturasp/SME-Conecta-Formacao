using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoObterTurmasInscricao
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar(long propostaId, string? codigoDre = null, bool comCodaf = false);
    }
}
