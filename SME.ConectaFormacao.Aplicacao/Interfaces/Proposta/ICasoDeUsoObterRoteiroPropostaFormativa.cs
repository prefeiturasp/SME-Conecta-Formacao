using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterRoteiroPropostaFormativa
    {
        Task<RoteiroPropostaFormativaDTO> Executar();
    }
}
