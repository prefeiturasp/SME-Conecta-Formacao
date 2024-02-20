using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterPropostaRegentePaginacao
    {
        Task<PaginacaoResultadoDTO<PropostaRegenteDTO>> Executar(long id);
    }
}