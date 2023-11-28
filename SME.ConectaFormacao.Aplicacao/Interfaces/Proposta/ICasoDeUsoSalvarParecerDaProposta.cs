using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoSalvarParecerDaProposta
    {
        Task<bool> Executar(long propostaId, PropostaMovimentacaoDTO propostaMovimentacaoDto);
    }
}