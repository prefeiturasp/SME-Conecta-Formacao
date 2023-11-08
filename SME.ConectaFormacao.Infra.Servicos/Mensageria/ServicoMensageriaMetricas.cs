using Newtonsoft.Json;
using Polly;
using Polly.Registry;
using RabbitMQ.Client;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Polly;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using SME.ConectaFormacao.Infra.Servicos.Telemetria;
using System.Text;

namespace SME.ConectaFormacao.Infra.Servicos.Mensageria
{
    public class ServicoMensageriaMetricas : IServicoMensageriaMetricas
    {
        private readonly IConexoesRabbit conexaoRabbit;
        private readonly IServicoTelemetria servicoTelemetria;
        private readonly IAsyncPolicy policy;

        public ServicoMensageriaMetricas(IConexoesRabbit conexaoRabbit, IServicoTelemetria servicoTelemetria, IReadOnlyPolicyRegistry<string> registry)
        {
            this.conexaoRabbit = conexaoRabbit ?? throw new ArgumentNullException(nameof(conexaoRabbit));
            this.servicoTelemetria = servicoTelemetria ?? throw new ArgumentNullException(nameof(servicoTelemetria));
            this.policy = registry.Get<IAsyncPolicy>(ConstsPoliticaPolly.PublicaFila);
        }

        public Task Concluido(string rota)
            => PublicarMetrica(TipoAcaoMensageria.Ack, rota);

        public Task Erro(string rota)
            => PublicarMetrica(TipoAcaoMensageria.Rej, rota);

        public Task Obtido(string rota)
            => PublicarMetrica(TipoAcaoMensageria.Get, rota);

        public Task Publicado(string rota)
            => PublicarMetrica(TipoAcaoMensageria.Pub, rota);

        public async Task<bool> Publicar(MetricaMensageria request, string rota, string exchange, string nomeAcao, IModel canalRabbit = null)
        {
            var mensagem = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var body = Encoding.UTF8.GetBytes(mensagem);

            Func<Task> fnTaskPublicarMensagem = async () => await PublicarMensagem(rota, body, exchange, canalRabbit);
            Func<Task> fnTaskPolicy = async () => await policy.ExecuteAsync(fnTaskPublicarMensagem);
            await servicoTelemetria.RegistrarAsync(fnTaskPolicy, "RabbitMQ", nomeAcao,rota);
            return true;
        }
        private Task PublicarMensagem(string rota, byte[] body, string exchange = null, IModel canalRabbit = null)
        {
            var channel = canalRabbit ?? conexaoRabbit.Get();
            try
            {
                var props = channel.CreateBasicProperties();
                props.Persistent = true;
                channel.BasicPublish(exchange, rota, true, props, body);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
            finally
            {
                conexaoRabbit.Return(channel);
            }
        }
        private Task PublicarMetrica(TipoAcaoMensageria tipoAcao, string rota)
                        => Publicar(new MetricaMensageria(tipoAcao.ToString(), rota),
                                    RotasRabbitLogs.RotaMetricas,
                                    ExchangeRabbit.QueueLogs,
                                    "Publicar Metrica Mensageria");
    }
}
