using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias
{
    public interface ICasoDeUsoListarCodafConferencia
    {
        Task<Resultado<PaginacaoResultadoDto<CodafConferenciaResumoDto>>> ExecutarAsync(FiltroCodafConferenciaDto filtro);
    }
}
