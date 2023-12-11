using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Opcoes;

namespace SME.ConectaFormacao.Infra.Servicos.Armazenamento.IoC
{
    public static class ServicoArmazenamentoCHelper
    {
        public static void ConfigurarArmazenamento(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ConfiguracaoArmazenamentoOptions>()
                .Bind(configuration.GetSection(ConfiguracaoArmazenamentoOptions.Secao), c => c.BindNonPublicProperties = true);

            services.AddSingleton<ConfiguracaoArmazenamentoOptions>();
            services.AddSingleton<IServicoArmazenamento, ServicoArmazenamento>();
        }
    }
}
