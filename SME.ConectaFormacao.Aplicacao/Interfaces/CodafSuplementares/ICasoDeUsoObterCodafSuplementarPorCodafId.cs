using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares
{
    public interface ICasoDeUsoObterCodafSuplementarPorCodafId
    {
        Task<Resultado<CodafSuplementarDetalhadoDto>> ExecutarAsync(long codafId);
    }
}
