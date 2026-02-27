using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados
{
    public interface ICasoDeUsoEmitirCertificadoCodaf
    {
        Task<Resultado> ExecutarAsync(long codafListaPresencaId);
    }
}
