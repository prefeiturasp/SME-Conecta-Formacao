using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Log;
using System.Diagnostics;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Middlewares
{
    public class TratamentoExcecaoGlobalMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IServicoLogs servicoLogs;

        public TratamentoExcecaoGlobalMiddleware(RequestDelegate next, IServicoLogs servicoLogs)
        {
            this.next = next;
            this.servicoLogs = servicoLogs ?? throw new ArgumentNullException(nameof(servicoLogs));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (NegocioException nex)
            {
                var mensagem = nex.Mensagens.Any() ? string.Join(" - ", nex.Mensagens) : nex.Message;

                await servicoLogs.Enviar(mensagem, observacao: nex.Message, rastreamento: nex.StackTrace);
                await TratarExcecao(context, nex, nex.StatusCode, nex.Mensagens.ToArray());
            }
            catch (Exception ex)
            {
                var mensagem = "Houve um comportamento inesperado do Conecta Formação. Por favor, contate a SME.";

                await servicoLogs.Enviar(mensagem, observacao: ex.Message, rastreamento: ex.StackTrace);
                await TratarExcecao(context, ex, mensagens: mensagem);
            }
        }

        private static async Task TratarExcecao(HttpContext context, Exception exception, int statusCode = (int)HttpStatusCode.InternalServerError, params string[] mensagens)
        {

#if DEBUG
            if (exception != null)
                Debug.WriteLine(exception);
#endif

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(new RetornoBaseDTO(mensagens.ToList()).ObjetoParaJson());
        }
    }

    public static class TratamentoExcecaoGlobalMiddlewareExtensions
    {
        public static IApplicationBuilder UseTratamentoExcecoesGlobalMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TratamentoExcecaoGlobalMiddleware>();
        }
    }
}
