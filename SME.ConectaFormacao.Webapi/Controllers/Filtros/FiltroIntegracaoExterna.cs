
using Microsoft.OpenApi.Models;
using SME.ConectaFormacao.Webapi.Middlewares;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SME.ConectaFormacao.Webapi.Controllers.Filtros
{
    public class FiltroIntegracaoExterna : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var atributos = context.MethodInfo.DeclaringType?
            .GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .OfType<ChaveIntegracaoApi>();

            if (atributos is null || !atributos.Any())
                return;

            operation.Parameters ??= []; 
            
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "x-conecta-formacao-api-key",
                In = ParameterLocation.Header,
                Required = false,
                Description = "Chave de API para integração externa (SME)"
            });
        }
    }
}
