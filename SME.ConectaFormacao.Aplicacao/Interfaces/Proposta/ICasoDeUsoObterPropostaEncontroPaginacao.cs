using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.PropostaEncontros;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterPropostaEncontroPaginacao
    {
        Task<PaginacaoResultadoDto<CronogramaEncontroDto>> ExecutarAsync(long id);
    }
}
