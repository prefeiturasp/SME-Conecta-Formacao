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
        public static void ConfigurarServicoAcessos(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
                return;

            var servicoAcessosOptions = new ServicoAcessosOptions();
            configuration.GetSection(ServicoAcessosOptions.Secao).Bind(servicoAcessosOptions, c => c.BindNonPublicProperties = true);
            services.AddSingleton(servicoAcessosOptions);

            services.AddHttpClient<IServicoAcessos, ServicoAcessos>(c =>
            {
                c.BaseAddress = new Uri(servicoAcessosOptions.UrlApi);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("x-api-acessos-key", servicoAcessosOptions.KeyApi);
            }).AddPolicyHandler(GetRetryPolicy());
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt)));
        }
    }
}

