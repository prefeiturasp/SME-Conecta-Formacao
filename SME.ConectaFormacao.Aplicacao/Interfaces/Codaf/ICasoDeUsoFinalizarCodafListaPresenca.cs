using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoFinalizarCodafListaPresenca
    {
        Task<Resultado> ExecutarAsync(long codafListaPresencaId);
    }
}
