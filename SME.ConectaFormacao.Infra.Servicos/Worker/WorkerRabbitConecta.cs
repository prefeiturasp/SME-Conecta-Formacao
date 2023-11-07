using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Telemetria;
using SME.ConectaFormacao.Infra.Servicos.Telemetria.Options;

namespace SME.ConectaFormacao.Infra
{
    public abstract class WorkerRabbitConecta : IHostedService
    {
        protected readonly IModel canalRabbit;
        private readonly IConnection conexaoRabbit;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IServicoTelemetria servicoTelemetria;
        private readonly IServicoMensageriaConecta servicoMensageria;
        private readonly IServicoMensageriaMetricas servicoMensageriaMetricas;
        private readonly TelemetriaOptions telemetriaOptions;
        private readonly string apmTransactionType;
        private readonly Type tipoRotas;
        private readonly bool retryAutomatico;

        protected WorkerRabbitConecta(IServiceScopeFactory serviceScopeFactory,
            IServicoTelemetria servicoTelemetria,
            IServicoMensageriaConecta servicoMensageria,
            IServicoMensageriaMetricas servicoMensageriaMetricas,
            IOptions<TelemetriaOptions> telemetriaOptions,
            IOptions<ConsumoFilasOptions> consumoFilasOptions,
            IConnectionFactory factory,
            string apmTransactionType,
            Type tipoRotas,
            bool retryAutomatico = true)
        {
            this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory), "Service Scope Factory não localizado");
            this.servicoTelemetria = servicoTelemetria ?? throw new ArgumentNullException(nameof(servicoTelemetria));
            this.servicoMensageria = servicoMensageria ?? throw new ArgumentNullException(nameof(servicoMensageria));
            this.servicoMensageriaMetricas = servicoMensageriaMetricas ?? throw new ArgumentNullException(nameof(servicoMensageriaMetricas));
            this.telemetriaOptions = telemetriaOptions.Value ?? throw new ArgumentNullException(nameof(telemetriaOptions));

            if (consumoFilasOptions.EhNulo())
                throw new ArgumentNullException(nameof(consumoFilasOptions));

            this.apmTransactionType = apmTransactionType ?? "WorkerRabbitConecta";
            this.tipoRotas = tipoRotas ?? throw new ArgumentNullException(nameof(tipoRotas));
            this.retryAutomatico = retryAutomatico;
            conexaoRabbit = factory.CreateConnection();
            canalRabbit = conexaoRabbit.CreateModel();
            canalRabbit.BasicQos(0, consumoFilasOptions.Value.Qos, false);
            canalRabbit.ExchangeDeclare(ExchangeRabbit.Conecta, ExchangeType.Direct, true, false);
            canalRabbit.ExchangeDeclare(ExchangeRabbit.ConectaDeadLetter, ExchangeType.Direct, true, false);
            canalRabbit.ExchangeDeclare(ExchangeRabbit.Logs, ExchangeType.Direct, true, false);

            Comandos = new Dictionary<string, ComandoRabbit>();
            RegistrarUseCases();
            DeclararFilas();
        }

        protected Dictionary<string, ComandoRabbit> Comandos { get; }
        protected abstract void RegistrarUseCases();

        protected virtual void DeclararFilas()
        {
            DeclararFilasPorRota(ExchangeRabbit.Conecta, ExchangeRabbit.ConectaDeadLetter);
        }

        protected void DeclararFilasPorRota(string exchange, string exchangeDeadLetter = "")
        {
            foreach (var fila in tipoRotas.ObterConstantesPublicas<string>())
            {
                var args = ObterArgumentoDaFila(fila, exchangeDeadLetter);
                canalRabbit.QueueDeclare(fila, true, false, false, args);
                canalRabbit.QueueBind(fila, exchange, fila, null);

                if (!string.IsNullOrEmpty(exchangeDeadLetter))
                {
                    var argsDlq = ObterArgumentoDaFilaDeadLetter(fila, exchange);
                    var filaDeadLetter = $"{fila}.deadletter";

                    canalRabbit.QueueDeclare(filaDeadLetter, true, false, false, argsDlq);
                    canalRabbit.QueueBind(filaDeadLetter, exchangeDeadLetter, fila, null);

                    if (retryAutomatico)
                    {
                        var argsLimbo = new Dictionary<string, object>();
                        argsLimbo.Add("x-queue-mode", "lazy");

                        var queueLimbo = $"{fila}.limbo";
                        canalRabbit.QueueDeclare(
                            queue: queueLimbo,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: argsLimbo
                        );

                        canalRabbit.QueueBind(queueLimbo, exchangeDeadLetter, queueLimbo, null);
                    }
                }
            }
        }
        private Dictionary<string, object> ObterArgumentoDaFilaDeadLetter(string fila, string exchange)
        {
            var argsDlq = new Dictionary<string, object>();

            argsDlq.Add("x-queue-mode", "lazy");
            if (retryAutomatico)
            {
                var ttl = Comandos.ContainsKey(fila) ? Comandos[fila].TTL : ExchangeRabbit.SgpDeadLetterTTL;
                argsDlq.Add("x-message-ttl", ttl);
                argsDlq.Add("x-dead-letter-exchange", exchange);
            }

            return argsDlq;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
