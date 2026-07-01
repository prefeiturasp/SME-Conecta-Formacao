using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares
{
    public interface ICasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar
    {
        Task<Resultado<ArquivoDto>> ExecutarAsync(long codafSuplementarId);
    }
}
