using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterPropostaPaginacao
    {
        Task<PaginacaoResultadoDto<PropostaPaginadaDTO>> Executar(PropostaFiltrosDTO propostaFiltrosDTO);
    }
}
