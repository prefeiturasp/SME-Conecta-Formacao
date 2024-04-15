using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoDevolverProposta
    {
        Task<bool> Executar(long propostaId, DevolverPropostaDTO devolverPropostaDto);
    }
}