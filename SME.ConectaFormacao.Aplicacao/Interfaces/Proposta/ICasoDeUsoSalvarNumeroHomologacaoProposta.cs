using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoSalvarNumeroHomologacaoProposta
    {
        Task<bool> Executar(long propostaId, PropostaNumeroHomologacaoDto dto);
    }
}
