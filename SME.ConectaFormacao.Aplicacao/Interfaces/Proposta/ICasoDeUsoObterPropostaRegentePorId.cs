using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterPropostaRegentePorId
    {
        Task<PropostaRegenteDTO> Executar(long regenteId);
    }
}