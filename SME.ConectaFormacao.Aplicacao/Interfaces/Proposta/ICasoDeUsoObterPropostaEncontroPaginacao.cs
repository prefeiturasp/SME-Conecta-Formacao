using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterPropostaEncontroPaginacao
    {
        Task<PaginacaoResultadoDto<PropostaEncontroDto>> Executar(long id);
    }
}
