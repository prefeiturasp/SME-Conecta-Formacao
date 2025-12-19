using Elastic.Apm;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Servicos.Log;
using System.Net;
using System.Text.Json;

namespace SME.ConectaFormacao.Webapi.Middlewares
{
    public class TratamentoExcecaoGlobalMiddleware(RequestDelegate next, IServicoLogs servicoLogs, IWebHostEnvironment env)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (NegocioException nex)
            {
                var mensagem = nex.Mensagens.Count != 0 ? string.Join(" - ", nex.Mensagens) : nex.Message;

                await servicoLogs.Enviar(mensagem, observacao: nex.Message, rastreamento: nex.StackTrace);
                await TratarExcecao(context, nex, nex.StatusCode, nex.Mensagens.ToArray());
            }
            catch (Exception ex)
            {
                if (env.IsDevelopment())
                {
                    throw;
                }
                var mensagem = "Houve um comportamento inesperado do Conecta Formação. Por favor, contate a SME.";

                await servicoLogs.Enviar(mensagem, observacao: ex.Message, rastreamento: ex.StackTrace);
                Agent.Tracer.CurrentTransaction?.CaptureException(ex);

                await TratarExcecao(context, ex, mensagens: mensagem);
            }
        }

        private static async Task TratarExcecao(HttpContext context, Exception exception, int statusCode = (int)HttpStatusCode.InternalServerError, params string[] mensagens)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new RetornoBaseDTO(mensagens.ToList()), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
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
