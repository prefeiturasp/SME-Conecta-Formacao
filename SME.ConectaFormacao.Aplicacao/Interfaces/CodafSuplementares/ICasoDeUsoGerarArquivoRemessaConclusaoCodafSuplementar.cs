using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares
{
    public interface ICasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar
    {
        Task<Resultado<CodafSuplementarArquivoDto>> ExecutarAsync(long codafSuplementarId);
    }
}
