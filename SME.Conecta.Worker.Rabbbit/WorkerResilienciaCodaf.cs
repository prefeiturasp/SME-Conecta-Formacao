using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados;

namespace SME.Conecta.Worker.Rabbbit
{
    public class WorkerResilienciaCodaf(IServiceScopeFactory serviceScopeFactory, ILogger<WorkerResilienciaCodaf> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Worker de Resiliência CODAF iniciado.");
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    try
                    {
                        var casoDeUso = scope.ServiceProvider.GetRequiredService<ICasoDeUsoRecuperarCertificadosTravadosCodafResiliencia>();

                        await casoDeUso.ExecutarAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogCritical(ex, "Falha crítica ao executar ciclo de resiliência.");
                    }
                }

                // Aguarda 10 minutos antes da próxima execução
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
