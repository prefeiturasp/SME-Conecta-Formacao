using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados
{
    public interface ICasoDeUsoObterCertificadoCodafParaDownload
    {
        Task<Resultado<CodafCertificadoParaDownloadDto>> ExecutarAsync(long certificadoCodafId);
    }
}
