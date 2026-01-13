using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoExcluirCodafListaPresenca
    {
        Task<Resultado> ExecutarAsync(long codafListaPresencaId);
    }
}