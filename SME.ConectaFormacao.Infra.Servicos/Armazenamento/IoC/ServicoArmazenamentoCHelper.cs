using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Opcoes;

namespace SME.ConectaFormacao.Infra.Servicos.Armazenamento.IoC
{
    public static class ServicoArmazenamentoCHelper
    {
        public static void ConfigurarArmazenamento(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ConfiguracaoArmazenamentoOptions>()
                .Bind(configuration.GetSection(ConfiguracaoArmazenamentoOptions.Secao), c => c.BindNonPublicProperties = true)
                .ValidateDataAnnotations();
            services.AddSingleton<IMinioClient>(sp =>
            {
                var opcoesWrapper = sp.GetRequiredService<IOptions<ConfiguracaoArmazenamentoOptions>>();
                var opcoes = opcoesWrapper.Value;
                if (opcoes == null) throw new InvalidOperationException("As configurações do MinIO não foram carregadas.");
                if (opcoes.Port == 0) throw new ArgumentException("A porta do MinIO não foi configurada corretamente (está 0). Verifique o appsettings ou as variáveis de ambiente.");
                return new MinioClient()
                    .WithEndpoint(opcoes.EndPoint, opcoes.Port)
                    .WithCredentials(opcoes.AccessKey, opcoes.SecretKey)
                    .WithSSL()
                    .Build();
            });

            services.AddSingleton<IServicoArmazenamento, ServicoArmazenamento>();
        }
    }
}
