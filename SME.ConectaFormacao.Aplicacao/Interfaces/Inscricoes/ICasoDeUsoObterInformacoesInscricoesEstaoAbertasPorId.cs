using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoObterInformacoesInscricoesEstaoAbertasPorId
    {
        Task<PodeInscreverMensagemDTO> Executar(long propostaId);
    }
}