using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Infra.Servicos.Mensageria.Options;

namespace SME.ConectaFormacao.Infra.Servicos.Mensageria.IoC
{
    public static class MensageriaIoCHelper
    {
        public static void ConfigurarMensageria(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
                return;

            services.AddOptions<MensageriaOptions>()
                .Bind(configuration.GetSection(MensageriaOptions.Secao), c => c.BindNonPublicProperties = true);

            services.AddTransient<MensageriaOptions>();
            services.AddTransient<IServicoMensageriaLogs, ServicoMensageriaLogs>();
            services.AddTransient<IServicoMensageriaConecta, ServicoMensageriaConecta>();
            services.AddTransient<IServicoMensageriaMetricas, ServicoMensageriaMetricas>();
        }
    }
}
