using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementar;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementar
{
    public interface ICasoDeUsoObterCodafSuplementarPorCodafId
    {
        Task<Resultado<CodafSuplementarDetalhadoDto>> ExecutarAsync(long codafId);
    }
}
