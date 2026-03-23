using Microsoft.Extensions.Logging;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados
{
    public class CasoDeUsoRecuperarCertificadosTravadosCodafResiliencia(
        IRepositorioCodafCertificado repositorioCodafCertificado,
        ILogger<CasoDeUsoRecuperarCertificadosTravadosCodafResiliencia> logger) : ICasoDeUsoRecuperarCertificadosTravadosCodafResiliencia
    {
        public async Task ExecutarAsync(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Iniciando recuperação de certificados travados CODAF...");
                await repositorioCodafCertificado.RecuperarCertificadosTravadosAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao recuperar certificados travados CODAF.");
            }
        }
    }
}
