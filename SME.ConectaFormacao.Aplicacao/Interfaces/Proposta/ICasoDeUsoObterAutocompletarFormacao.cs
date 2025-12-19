using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterAutocompletarFormacao
    {
        Task<Resultado<PaginacaoResultadoDto<AutocompletarNumeroHomologacaoDto>>> ExecutarAsync(FiltroAutocompletarNumeroHomologacaoDto filtro);
    }
}