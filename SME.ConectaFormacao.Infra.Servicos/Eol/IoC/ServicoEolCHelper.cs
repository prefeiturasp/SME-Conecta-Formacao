using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol.Options;

namespace SME.ConectaFormacao.Infra.Servicos.Eol.IoC
{
    public static class ServicoEolCHelper
    {
        public static void ConfigurarServicoEol(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
                return;
            var servicoEolOptions = new ServicoEolOptions();
            configuration.GetSection(ServicoEolOptions.Secao).Bind(servicoEolOptions, c => c.BindNonPublicProperties = true);
            services.AddSingleton(servicoEolOptions);

            services.AddHttpClient<IServicoEol, ServicoEol>(c =>
            {
                c.BaseAddress = new Uri(servicoEolOptions.UrlApiEol);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("x-api-eol-key", servicoEolOptions.ChaveIntegracaoApi);
                if (configuration.GetSection("HttpClientTimeoutSecond").Value.NaoEhNulo())
                    c.Timeout = TimeSpan.FromSeconds(double.Parse(configuration.GetSection("HttpClientTimeoutSecond").Value));
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