using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace SME.ConectaFormacao.Webapi.Middlewares
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ChaveIntegracaoCdepApi : Attribute, IAsyncActionFilter
    {
        private const string ChaveIntegracaoHeader = "x-cdep-api-key";
        private const string ChaveIntegracaoEnvironmentVariableName = "ChaveIntegracaoApi";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string chaveApi = Environment.GetEnvironmentVariable(ChaveIntegracaoEnvironmentVariableName);

            if (!context.HttpContext.Request.Headers.TryGetValue(ChaveIntegracaoHeader, out var chaveRecebida) ||
                !chaveRecebida.Equals(chaveApi))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}
