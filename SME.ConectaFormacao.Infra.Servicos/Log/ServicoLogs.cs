using Polly;
using Polly.Registry;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Polly;
using System.Text;

namespace SME.ConectaFormacao.Infra.Servicos.Log
{
    public class ServicoLogs(IConexoesRabbitLogs conexoesRabbit, IReadOnlyPolicyRegistry<string> registry) : IServicoLogs
    {
        private readonly IAsyncPolicy policy = registry.Get<IAsyncPolicy>(ConstsPoliticaPolly.PublicaFila);

        public async Task Enviar(string mensagem, LogContexto contexto = LogContexto.Geral, LogNivel nivel = LogNivel.Critico, string observacao = "", string? rastreamento = "")
        {
            var logMensagem = new LogMensagem(mensagem, contexto.ToString(), nivel.ToString(), observacao, rastreamento).ObjetoParaJson();
            var body = Encoding.UTF8.GetBytes(logMensagem);

            await policy.ExecuteAsync(async ()
                => await PublicarMensagem(body));
        }

        public async Task Enviar(Exception ex, string mensagem, LogContexto contexto = LogContexto.Geral, LogNivel nivel = LogNivel.Critico, string observacao = "")
        {
            var rastreamento = MontarRastreamento(ex);
            var excecaoCompleta = MontarExcecaoCompleta(ex);
            var logMensagem = new LogMensagem(mensagem, contexto.ToString(), nivel.ToString(), observacao, rastreamento, excecaoCompleta).ObjetoParaJson();
            var body = Encoding.UTF8.GetBytes(logMensagem);

            await policy.ExecuteAsync(async ()
                => await PublicarMensagem(body));
        }

        private static string MontarExcecaoCompleta(Exception ex)
        {
            var excecaoCompleta = new StringBuilder();
            var exceptionAtual = ex;
            while (exceptionAtual != null)
            {
                excecaoCompleta.AppendLine($"Exception Type: {exceptionAtual.GetType().FullName}");
                excecaoCompleta.AppendLine($"Message: {exceptionAtual.Message}");
                excecaoCompleta.AppendLine($"StackTrace: {exceptionAtual.StackTrace ?? string.Empty}");
                exceptionAtual = exceptionAtual.InnerException;
            }
            return excecaoCompleta.ToString();
        }

        private static string MontarRastreamento(Exception ex)
        {
            var rastreamento = new StringBuilder();
            var exceptionAtual = ex;
            while (exceptionAtual != null)
            {
                rastreamento.AppendLine(exceptionAtual.Message);
                rastreamento.AppendLine(exceptionAtual.StackTrace ?? string.Empty);
                exceptionAtual = exceptionAtual.InnerException;
            }
            return rastreamento.ToString();
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
