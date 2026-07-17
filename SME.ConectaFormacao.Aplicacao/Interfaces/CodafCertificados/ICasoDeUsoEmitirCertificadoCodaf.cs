using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados
{
    public interface ICasoDeUsoEmitirCertificadoCodaf
    {
        Task<Resultado> ExecutarAsync(long codafId, TipoCodaf tipoCodaf);
    }
}
