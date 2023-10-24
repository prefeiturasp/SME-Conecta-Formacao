using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoSalvarPropostaRegente
    {
        Task<long> Executar(long id,  PropostaRegenteDTO propostaRegenteDto);
    }
}