using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterPropostaParecer
    {
        Task<PropostaPareceristaConsideracaoCompletoDTO> Executar(PropostaParecerFiltroDTO propostaParecerFiltroDto);
    }
}