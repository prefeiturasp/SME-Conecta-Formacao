using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Servicos.Log;
using System.Net;
using Elastic.Apm;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Webapi.Filtros
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class PadronizarRetornoFiltroAttribute : ActionFilterAttribute, IExceptionFilter
    {
        private const string ApplicationJson = "application/json";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Validação automática de payload (400 Bad Request)
            if (!context.ModelState.IsValid)
            {
                var erros = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var responseObj = new
                {
                    sucesso = false,
                    erros
                };

                context.Result = new ObjectResult(responseObj)
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ContentTypes = { ApplicationJson }
                };
            }

            base.OnActionExecuting(context);
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.StatusCode >= 400)
                {
                    objectResult.ContentTypes.Clear();
                    objectResult.ContentTypes.Add(ApplicationJson);
                }
            }
            else if (context.Result is StatusCodeResult statusCodeResult && statusCodeResult.StatusCode >= 400)
            {
                var responseObj = new
                {
                    sucesso = false,
                    erros = new[] { ObterMensagemPadrao(statusCodeResult.StatusCode) }
                };

                context.Result = new ObjectResult(responseObj)
                {
                    StatusCode = statusCodeResult.StatusCode,
                    ContentTypes = { ApplicationJson }
                };
            }

            base.OnResultExecuting(context);
        }

        public void OnException(ExceptionContext context)
        {
            var servicoLogs = context.HttpContext.RequestServices.GetService<IServicoLogs>();
            var env = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>();

            int statusCode = (int)HttpStatusCode.InternalServerError;
            var mensagens = new List<string>();

            if (context.Exception is NegocioException nex)
            {
                statusCode = nex.StatusCode;
                mensagens.AddRange(nex.Mensagens.Count != 0 ? nex.Mensagens : new[] { nex.Message });

                _ = servicoLogs?.Enviar(string.Join(" - ", mensagens), observacao: nex.Message, rastreamento: nex.StackTrace);
            }
            else
            {
                if (env != null && env.IsDevelopment())
                {
                    return;
                }

                var msgErro = "Houve um comportamento inesperado do Conecta Formação. Por favor, contate a SME.";
                mensagens.Add(msgErro);

                _ = servicoLogs?.Enviar(msgErro, observacao: context.Exception.Message, rastreamento: context.Exception.StackTrace);
                Agent.Tracer.CurrentTransaction?.CaptureException(context.Exception);
            }

            var responseObj = new
            {
                sucesso = false,
                erros = mensagens
            };

            context.Result = new ObjectResult(responseObj)
            {
                StatusCode = statusCode,
                ContentTypes = { ApplicationJson }
            };

            context.ExceptionHandled = true;
        }

        private static string ObterMensagemPadrao(int statusCode)
        {
            return statusCode switch
            {
                400 => "Requisição inválida.",
                401 => "Não autenticado.",
                403 => "Acesso negado.",
                404 => "Recurso não encontrado.",
                422 => "Entidade não processável.",
                500 => "Erro interno no servidor.",
                _ => "Ocorreu um erro inesperado."
            };
        }
    }
}
