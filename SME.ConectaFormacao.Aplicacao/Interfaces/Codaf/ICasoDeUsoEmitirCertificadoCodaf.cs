using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoEmitirCertificadoCodaf
    {
        Task<Resultado> ExecutarAsync(long codafListaPresencaId);
    }
}
