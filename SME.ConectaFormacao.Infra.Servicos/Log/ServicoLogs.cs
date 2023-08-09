using Polly;
using Polly.Registry;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Polly;
using System.Text;

namespace SME.ConectaFormacao.Infra.Servicos.Log
{
    public class ServicoLogs : IServicoLogs
    {
        private readonly IConexoesRabbitLogs conexoesRabbit;
        private readonly IAsyncPolicy policy;

        public ServicoLogs(IConexoesRabbitLogs conexoesRabbit, IReadOnlyPolicyRegistry<string> registry)
        {
            this.conexoesRabbit = conexoesRabbit ?? throw new ArgumentNullException(nameof(conexoesRabbit));
            this.policy = registry.Get<IAsyncPolicy>(ConstsPoliticaPolly.PublicaFila);
        }

        public async Task Enviar(string mensagem, LogContexto contexto = LogContexto.Geral, LogNivel nivel = LogNivel.Critico, string observacao = "", string rastreamento = "")
        {
            var logMensagem = new LogMensagem(mensagem, contexto.ToString(), nivel.ToString(), observacao, rastreamento).ObjetoParaJson();
            var body = Encoding.UTF8.GetBytes(logMensagem);

            await policy.ExecuteAsync(async ()
                => await PublicarMensagem(body));
        }

        private Task PublicarMensagem(byte[] body)
        {
            var channel = conexoesRabbit.Get();
            try
            {
                var props = channel.CreateBasicProperties();
                props.Persistent = true;

                channel.BasicPublish(ExchangeRabbit.Logs, RotasRabbitLogs.RotaLogs, true, props, body);
            }
            finally
            {
                conexoesRabbit.Return(channel);
            }

            return Task.CompletedTask;
        }
    }
}
