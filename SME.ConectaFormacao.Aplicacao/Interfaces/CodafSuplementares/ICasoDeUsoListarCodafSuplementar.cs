using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares
{
    public interface ICasoDeUsoListarCodafSuplementar
    {
        Task<Resultado<PaginacaoResultadoDto<CodafSuplementarResumoDto>>> ExecutarAsync(FiltroCodafSuplementarDto filtro);
    }
}