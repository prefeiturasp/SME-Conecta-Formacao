using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterNumeroHomologacaoProposta
    {
        Task<Resultado<PropostaNumeroHomologacaoDto>> ExecutarAsync(long propostaId);
    }
}
