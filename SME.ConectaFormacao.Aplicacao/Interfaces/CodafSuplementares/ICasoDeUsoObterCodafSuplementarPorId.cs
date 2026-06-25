using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares
{
    public interface ICasoDeUsoObterCodafSuplementarPorId
    {
        Task<Resultado<CodafSuplementarDetalhadoDto>> ExecutarAsync(long codafSuplementarId);
    }
}
