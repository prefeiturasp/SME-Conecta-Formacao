using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterFormacaoDetalhada
    {
        Task<RetornoFormacaoDetalhadaDTO> Executar(long propostaId);
    }
}
