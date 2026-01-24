using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoGerarArquivoDeInscricoesDoCodafParaEol
    {
        Task<Resultado<ArquivoDto>> ExecutarAsync(long codafListaPresencaId);
    }
}
