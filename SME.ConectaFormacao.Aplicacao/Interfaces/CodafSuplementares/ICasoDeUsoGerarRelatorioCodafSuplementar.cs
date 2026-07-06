using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares
{
    public interface ICasoDeUsoGerarRelatorioCodafSuplementar
    {
        Task<Resultado<ArquivoDto>> ExecutarAsync(long codafListaPresencaId);
    }
}
