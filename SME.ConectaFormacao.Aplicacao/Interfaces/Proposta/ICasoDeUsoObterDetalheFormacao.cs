using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterDetalheFormacao
    {
        Task<RetornoDetalheFormacaoDTO> Executar(long propostaId);
    }
}
