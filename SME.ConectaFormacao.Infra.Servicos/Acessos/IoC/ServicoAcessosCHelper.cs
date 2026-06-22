using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Options;

namespace SME.ConectaFormacao.Infra.Servicos.Acessos.IoC
{
    public static class ServicoAcessosCHelper
    {
        public static IServiceCollection ConfigurarServicoAcessos(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
                return services;

            var servicoAcessosOptions = new ServicoAcessosOptions();
            configuration.GetSection(ServicoAcessosOptions.Secao).Bind(servicoAcessosOptions, c => c.BindNonPublicProperties = true);
            services.AddSingleton(servicoAcessosOptions);

            services.AddHttpClient<IServicoAcessos, ServicoAcessos>(c =>
            {
                c.BaseAddress = new Uri("https://localhost:7178/api/");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("x-api-acessos-key", servicoAcessosOptions.KeyApi);
            }).AddPolicyHandler(GetRetryPolicy());
            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt)));
        }
    }
}

