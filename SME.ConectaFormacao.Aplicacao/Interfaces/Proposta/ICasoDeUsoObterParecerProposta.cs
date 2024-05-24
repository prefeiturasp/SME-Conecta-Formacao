using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterParecerProposta
    {
        Task<PropostaMovimentacaoDTO> Executar(long propostaId);
    }
}
