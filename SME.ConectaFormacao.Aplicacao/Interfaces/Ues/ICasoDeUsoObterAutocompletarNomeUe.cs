using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.Ues;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Ues
{
    public interface ICasoDeUsoObterAutocompletarNomeUe
    {
        Task<Resultado<PaginacaoResultadoDto<AutocompletarNomeUeDto>>> ExecutarAsync(FiltroAutocompletarNomeUeDto filtro);
    }
}
