using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Options;

namespace SME.ConectaFormacao.Infra.Servicos.Relatorio.IoC
{
    public static class ServicoRelatorioCHelper
    {
        public static void ConfigurarServicoRelatorio(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
                return;
            var relatorioOptions = new RelatorioOptions();
            configuration.GetSection(RelatorioOptions.Secao).Bind(relatorioOptions, c => c.BindNonPublicProperties = true);
            services.AddSingleton(relatorioOptions);

            services.AddHttpClient<IServicoRelatorio, ServicoRelatorio>(c =>
            {
                c.BaseAddress = new Uri(relatorioOptions.UrlApiServidorRelatorios);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("x-sr-api-key", relatorioOptions.ApiKeySr);

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
