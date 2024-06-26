﻿using Elastic.Apm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Mensageria;
using SME.ConectaFormacao.Infra.Servicos.Telemetria;
using SME.ConectaFormacao.Infra.Servicos.Telemetria.Options;
using SME.ConectaFormacao.Infra.Servicos.Utilitarios;
using System.Text;

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
        private Dictionary<string, object> ObterArgumentoDaFila(string fila, string exchangeDeadLetter)
        {
            var args = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(exchangeDeadLetter))
                args.Add("x-dead-letter-exchange", exchangeDeadLetter);

            if (Comandos.ContainsKey(fila) && Comandos[fila].ModeLazy)
                args.Add("x-queue-mode", "lazy");

            return args;
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
        private ulong GetRetryCount(IBasicProperties properties)
        {
            if (properties.Headers.NaoEhNulo() && properties.Headers.ContainsKey("x-death"))
            {
                var deathProperties = (List<object>)properties.Headers["x-death"];
                var lastRetry = (Dictionary<string, object>)deathProperties[0];
                var count = lastRetry["count"];
                return (ulong)Convert.ToInt64(count);
            }
            else
            {
                return 0;
            }
        }
        public async Task TratarMensagem(BasicDeliverEventArgs ea)
        {
            var mensagem = Encoding.UTF8.GetString(ea.Body.Span);
            var rota = ea.RoutingKey;

            await servicoMensageriaMetricas.Obtido(rota);

            if (Comandos.ContainsKey(rota))
            {
                var mensagemRabbit = mensagem.JsonParaObjeto<MensagemRabbit>();
                var comandoRabbit = Comandos[rota];

                var transacao = telemetriaOptions.Apm ? Agent.Tracer.StartTransaction(rota, apmTransactionType) : null;
                try
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    AtribuirContextoAplicacao(mensagemRabbit, scope);

                    IRabbitUseCase casoDeUso = (IRabbitUseCase)scope.ServiceProvider.GetService(comandoRabbit.TipoCasoUso);

                    await servicoTelemetria.RegistrarAsync(
                            async () => await casoDeUso.Executar(mensagemRabbit),
                            "RabbitMQ",
                            rota,
                            rota,
                            mensagem);

                    canalRabbit.BasicAck(ea.DeliveryTag, false);
                    await servicoMensageriaMetricas.Concluido(rota);
                }
                catch (NegocioException nex)
                {
                    transacao?.CaptureException(nex);

                    canalRabbit.BasicReject(ea.DeliveryTag, false);
                    await servicoMensageriaMetricas.Erro(rota);

                    await RegistrarErroTratamentoMensagem(ea, mensagemRabbit, nex, LogNivel.Negocio, $"Erros: {nex.Message}");

                    if (mensagemRabbit.NotificarErroUsuario)
                        NotificarErroUsuario(nex.Message, mensagemRabbit.UsuarioLogadoRF, comandoRabbit.NomeProcesso);
                }
                catch (Exception ex)
                {
                    transacao?.CaptureException(ex);

                    var rejeicoes = GetRetryCount(ea.BasicProperties);
                    if (++rejeicoes >= comandoRabbit.QuantidadeReprocessamentoDeadLetter)
                    {
                        canalRabbit.BasicAck(ea.DeliveryTag, false);

                        var filaLimbo = $"{ea.RoutingKey}.limbo";
                        await servicoMensageria.Publicar(mensagemRabbit, filaLimbo, ExchangeRabbit.ConectaDeadLetter, "PublicarDeadLetter");
                    }
                    else canalRabbit.BasicReject(ea.DeliveryTag, false);

                    await servicoMensageriaMetricas.Erro(rota);
                    await RegistrarErroTratamentoMensagem(ea, mensagemRabbit, ex, LogNivel.Critico, $"Erros: {ex.Message}");

                    if (mensagemRabbit.NotificarErroUsuario)
                        NotificarErroUsuario($"Ocorreu um erro interno, por favor tente novamente", mensagemRabbit.UsuarioLogadoRF, comandoRabbit.NomeProcesso);
                }
                finally
                {
                    transacao?.End();
                }
            }
            else
            {
                canalRabbit.BasicReject(ea.DeliveryTag, false);
                await servicoMensageriaMetricas.Erro(rota);

                var mensagemRabbit = mensagem.JsonParaObjeto<MensagemRabbit>();
                await RegistrarErroTratamentoMensagem(ea, mensagemRabbit, null, LogNivel.Critico, $"Rota não registrada");
            }
        }

        protected virtual Task RegistrarErroTratamentoMensagem(BasicDeliverEventArgs ea, MensagemRabbit mensagemRabbit, Exception ex, LogNivel logNivel, string observacao)
        {
            return Task.CompletedTask;
        }

        protected virtual Task RegistrarErro(string mensagem, LogNivel logNivel, string observacao = "", string rastreamento = "", string excecaoInterna = "")
        {
            return Task.CompletedTask;
        }
        protected virtual void AtribuirContextoAplicacao(MensagemRabbit mensagemRabbit, IServiceScope scope)
        {
        }
        protected virtual void NotificarErroUsuario(string message, string usuarioRf, string nomeProcesso)
        {
        }
        public Task StartAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(canalRabbit);

            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    await TratarMensagem(ea);
                }
                catch (Exception ex)
                {
                    await RegistrarErro($"Erro ao tratar mensagem {ea.DeliveryTag} - {ea.RoutingKey}", LogNivel.Critico, ex.Message);
                    canalRabbit.BasicReject(ea.DeliveryTag, false);
                }
            };

            RegistrarConsumerConecta(consumer, tipoRotas);
            return Task.CompletedTask;
        }
        private void RegistrarConsumerConecta(EventingBasicConsumer consumer, Type tipoRotas)
        {
            foreach (var fila in tipoRotas.ObterConstantesPublicas<string>())
                canalRabbit.BasicConsume(fila, false, consumer);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            canalRabbit.Close();
            conexaoRabbit.Close();
            return Task.CompletedTask;
        }
    }
}
