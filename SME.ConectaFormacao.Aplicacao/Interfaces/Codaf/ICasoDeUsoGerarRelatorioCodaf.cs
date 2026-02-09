using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoGerarRelatorioCodaf
    {
        Task<Resultado<ArquivoDto>> ExecutarAsync(long codafId);
    }
}
