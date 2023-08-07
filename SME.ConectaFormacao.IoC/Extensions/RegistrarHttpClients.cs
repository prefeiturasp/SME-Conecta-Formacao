using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Infra.Servicos.Acessos.IoC;

namespace SME.ConectaFormacao.IoC.Extensions;

internal static class RegistrarHttpClients
    {
        internal static void AdicionarHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigurarServicoAcessos(configuration);
        }
    }