﻿using Microsoft.Win32;
using Newtonsoft.Json;
using Polly;
using Polly.Registry;
using RabbitMQ.Client;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Polly;
using SME.ConectaFormacao.Infra.Servicos.Telemetria;
using System.Text;

namespace SME.ConectaFormacao.Infra.Servicos.Mensageria
{
    public class ServicoMensageriaConecta : IServicoMensageriaConecta
    {
        private readonly IConexoesRabbit conexaoRabbit;
        private readonly IServicoTelemetria servicoTelemetria;
        private readonly IAsyncPolicy policy;

        public ServicoMensageriaConecta(IConexoesRabbit conexaoRabbit, IServicoTelemetria servicoTelemetria, IReadOnlyPolicyRegistry<string> registry)
        {
            this.conexaoRabbit = conexaoRabbit ?? throw new ArgumentNullException(nameof(conexaoRabbit));
            this.servicoTelemetria = servicoTelemetria ?? throw new ArgumentNullException(nameof(servicoTelemetria));
            this.policy = registry.Get<IAsyncPolicy>(ConstsPoliticaPolly.PublicaFila);
        }

        public async Task<bool> Publicar(MensagemRabbit request, string rota, string exchange, string nomeAcao, IModel canalRabbit = null)
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
    }
}
