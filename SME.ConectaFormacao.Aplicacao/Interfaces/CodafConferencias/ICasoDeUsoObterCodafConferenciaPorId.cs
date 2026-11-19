using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias
{
    public interface ICasoDeUsoObterCodafConferenciaPorId
    {
        Task<Resultado<CodafConferenciaDetalhadoDto>> ExecutarAsync(long codafConferenciaId);
    }
}
