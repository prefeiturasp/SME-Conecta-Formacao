using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoAprovarPropostaParecerista
    {
        Task<bool> Executar(long propostaId, PropostaJustificativaDTO propostaJustificativaDTO);
    }
}
