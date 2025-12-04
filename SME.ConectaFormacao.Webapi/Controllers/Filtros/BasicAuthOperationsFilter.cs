using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SME.ConectaFormacao.Webapi.Controllers.Filtros;

public class BasicAuthOperationsFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var noAuthRequired = context.ApiDescription.CustomAttributes()
            .Any(attr => attr.GetType() == typeof(AllowAnonymousAttribute));

        if (noAuthRequired)
            return;

        operation.Security =
        [
            new()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            }
        ];
    }
}