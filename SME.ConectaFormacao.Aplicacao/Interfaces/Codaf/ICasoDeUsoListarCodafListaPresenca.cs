using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoListarCodafListaPresenca
    {
        Task<Resultado<PaginacaoResultadoDto<ListaPresencaCodafResumoDto>>> ExecutarAsync(FiltroListaPresencaCodafDto filtro);
    }
}