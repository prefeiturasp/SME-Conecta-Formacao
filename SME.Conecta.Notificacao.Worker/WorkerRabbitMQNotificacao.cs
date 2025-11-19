using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SME.Conecta.Notificacao.Worker.Notificacao;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using SME.ConectaFormacao.Infra.Servicos.Telemetria;
using System.Reflection;
using System.Text;

namespace SME.Conecta.Notificacao.Worker
{
    public sealed class WorkerRabbitMQNotificacao : IHostedService
    {
        private readonly INotificacao _notificacao;
        private readonly IServicoTelemetria _servicoTelemetria;

        private readonly IConnection _conexaoRabbit;
        private readonly IModel _canalRabbit;

        private readonly Dictionary<string, string> _comandos = new();

        public WorkerRabbitMQNotificacao(INotificacao notificacao, IServicoTelemetria servicoTelemetria, IConnectionFactory factory)
        {
            _notificacao = notificacao ?? throw new ArgumentNullException(nameof(notificacao));
            _servicoTelemetria = servicoTelemetria ?? throw new ArgumentNullException(nameof(servicoTelemetria));

            factory = factory ?? throw new ArgumentNullException(nameof(factory));

            _conexaoRabbit = factory.CreateConnection();
            _canalRabbit = _conexaoRabbit.CreateModel();

            _canalRabbit.BasicQos(0, 10, false);

            _canalRabbit.ExchangeDeclare(WorkerRabbitMQNotificacaoConstantes.Exchange, ExchangeType.Direct, true, false);
            _canalRabbit.ExchangeDeclare(WorkerRabbitMQNotificacaoConstantes.ExchangeDeadLetter, ExchangeType.Direct, true, false);

            DeclararFilas();
            RegistrarComandos();
        }

        private void DeclararFilas()
        {
            DeclararFila(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoCriadaUsuarios);
            DeclararFila(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoLidaUsuarios);
            DeclararFila(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoExcluidaUsuarios);

            DeclararFila(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoCriadaGrupos);
            DeclararFila(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoLidaGrupos);
            DeclararFila(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoExcluidaGrupos);
        }

        private void DeclararFila(string fila)
        {
            var args = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", WorkerRabbitMQNotificacaoConstantes.ExchangeDeadLetter },
                { "x-queue-mode", "lazy" }
            };

            _canalRabbit.QueueDeclare(fila, true, false, false, args);
            _canalRabbit.QueueBind(fila, WorkerRabbitMQNotificacaoConstantes.Exchange, fila, null);

            var filaDeadLetter = $"{fila}.deadletter";

            var argsDl = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", WorkerRabbitMQNotificacaoConstantes.Exchange },
                { "x-queue-mode", "lazy" },
                { "x-message-ttl", WorkerRabbitMQNotificacaoConstantes.DeadLetterTtl }
            };

            _canalRabbit.QueueDeclare(filaDeadLetter, true, false, false, argsDl);
            _canalRabbit.QueueBind(filaDeadLetter, WorkerRabbitMQNotificacaoConstantes.ExchangeDeadLetter, fila, null);

            var filaDeadLetterLimbo = $"{fila}.deadletter.limbo";

            var argsDlLimbo = new Dictionary<string, object>
            {
                { "x-queue-mode", "lazy" },
            };

            _canalRabbit.QueueDeclare(filaDeadLetterLimbo, true, false, false, argsDlLimbo);
            _canalRabbit.QueueBind(filaDeadLetterLimbo, WorkerRabbitMQNotificacaoConstantes.ExchangeDeadLetter, filaDeadLetterLimbo, null);
        }

        private void RegistrarComandos()
        {
            _comandos.Add(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoCriadaUsuarios, nameof(_notificacao.NotificacaoCriadaUsuarios));
            _comandos.Add(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoLidaUsuarios, nameof(_notificacao.NotificacaoLidaUsuarios));
            _comandos.Add(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoExcluidaUsuarios, nameof(_notificacao.NotificacaoExcluidaUsuarios));
            _comandos.Add(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoCriadaGrupos, nameof(_notificacao.NotificacaoCriadaGrupos));
            _comandos.Add(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoLidaGrupos, nameof(_notificacao.NotificacaoLidaGrupos));
            _comandos.Add(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoExcluidaGrupos, nameof(_notificacao.NotificacaoExcluidaGrupos));
        }

        private static ulong ObterQuantidadeErros(IBasicProperties properties)
        {
            if (properties.Headers == null || !properties.Headers.ContainsKey("x-death"))
                return 0;

            var deathProperties = (List<object>)properties.Headers["x-death"];
            var lastRetry = (Dictionary<string, object>)deathProperties[0];
            var count = lastRetry["count"];

            return (ulong)Convert.ToInt64(count);
        }

        private static MethodInfo ObterMetodo(Type objType, string method)
        {
            var executar = objType.GetMethod(method);

            if (executar != null)
                return executar;

            foreach (var itf in objType.GetInterfaces())
            {
                executar = ObterMetodo(itf, method);

                if (executar != null)
                    break;
            }

            return executar;
        }

        private async Task TratarMensagem(BasicDeliverEventArgs ea)
        {
            var mensagem = Encoding.UTF8.GetString(ea.Body.Span);
            var rota = ea.RoutingKey;

            if (_comandos.ContainsKey(rota))
            {
                var transacao = _servicoTelemetria.Iniciar(rota, "WorkerRabbitMQNotificacao");
                try
                {
                    var mensagemRabbit = mensagem.JsonParaObjeto<MensagemRabbit>();
                    var comando = _comandos[rota];
                    var metodo = ObterMetodo(_notificacao.GetType(), comando);

                    await _servicoTelemetria.RegistrarAsync(
                        () => (Task)metodo.Invoke(_notificacao, new object[] { mensagemRabbit }),
                        "RabbitMQ",
                        rota,
                        comando,
                        mensagem);

                    _canalRabbit.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    transacao?.CaptureException(ex);

                    var rejeicoes = ObterQuantidadeErros(ea.BasicProperties);
                    if (++rejeicoes >= WorkerRabbitMQNotificacaoConstantes.QuantidadeReprocessamentoDeadLetter)
                    {
                        _canalRabbit.BasicAck(ea.DeliveryTag, false);
                    }
                    else
                        _canalRabbit.BasicReject(ea.DeliveryTag, false);
                }
                finally
                {
                    _servicoTelemetria.Finalizar(transacao);
                }
            }
        }

        private void RegistrarConsumer(EventingBasicConsumer consumer)
        {
            _canalRabbit.BasicConsume(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoCriadaUsuarios, false, consumer);
            _canalRabbit.BasicConsume(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoLidaUsuarios, false, consumer);
            _canalRabbit.BasicConsume(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoExcluidaUsuarios, false, consumer);
            _canalRabbit.BasicConsume(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoCriadaGrupos, false, consumer);
            _canalRabbit.BasicConsume(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoLidaGrupos, false, consumer);
            _canalRabbit.BasicConsume(WorkerRabbitMQNotificacaoConstantes.EnviarNotificacaoExcluidaGrupos, false, consumer);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_canalRabbit);

            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    await TratarMensagem(ea);
                }
                catch (Exception ex)
                {
                    _canalRabbit.BasicReject(ea.DeliveryTag, false);
                    Console.WriteLine($"*** ERRO: {ex.Message}");
                }
            };

            RegistrarConsumer(consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _canalRabbit.Close();
            _conexaoRabbit.Close();

            return Task.CompletedTask;
        }
    }
}
