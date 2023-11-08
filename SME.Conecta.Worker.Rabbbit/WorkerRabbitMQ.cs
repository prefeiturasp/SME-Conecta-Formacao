using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Mensageria;
using SME.ConectaFormacao.Infra.Servicos.Telemetria;
using SME.ConectaFormacao.Infra.Servicos.Telemetria.Options;

namespace SME.Conecta.Worker
{
    public sealed class WorkerRabbitMQ : WorkerRabbitConecta
    {
        public WorkerRabbitMQ(IServiceScopeFactory serviceScopeFactory,
            IServicoTelemetria servicoTelemetria,
            IServicoMensageriaConecta servicoMensageria,
            IServicoMensageriaMetricas servicoMensageriaMetricas,
            IOptions<TelemetriaOptions> telemetriaOptions,
            IOptions<ConsumoFilasOptions> consumoFilasOptions,
            IConnectionFactory factory)
            : base(serviceScopeFactory, servicoTelemetria, servicoMensageria, servicoMensageriaMetricas, telemetriaOptions, consumoFilasOptions, factory, "WorkerRabbitMQ", typeof(RotasRabbit))
        {
            RegistrarUseCases();
        }

        protected override void RegistrarUseCases()
        {
            Comandos.Add(RotasRabbit.SincronizaEstruturaInstitucionalDreSync, new ComandoRabbit("Estrutura Institucional - Obter Dre", typeof(IExecutarSincronizacaoInstitucionalDreSyncUseCase), true));
            Comandos.Add(RotasRabbit.SincronizaEstruturaInstitucionalDreTratar, new ComandoRabbit("Estrutura Institucional - Tratar uma Dre", typeof(IExecutarSincronizacaoInstitucionalDreTratarUseCase), true));
        }
    }
}