using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterPropostaPaginacao
    {
        Task<PaginacaoResultadoDTO<PropostaPaginadaDTO>> Executar(PropostaFiltrosDTO propostaFiltrosDTO);
    }
}
