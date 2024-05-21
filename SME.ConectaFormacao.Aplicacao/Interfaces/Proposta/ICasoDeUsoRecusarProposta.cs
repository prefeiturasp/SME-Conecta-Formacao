using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoRecusarProposta
    {
        Task<bool> Executar(long propostaId, PropostaJustificativaDTO propostaJustificativa);
    }
}
