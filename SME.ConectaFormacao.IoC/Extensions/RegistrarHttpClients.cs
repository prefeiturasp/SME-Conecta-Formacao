using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Infra.Servicos.Acessos.IoC;
using SME.ConectaFormacao.Infra.Servicos.Compactacao.Ioc;
using SME.ConectaFormacao.Infra.Servicos.Eol.IoC;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.IoC;

namespace SME.ConectaFormacao.IoC.Extensions;

internal static class RegistrarHttpClients
{
    internal static IServiceCollection AdicionarHttpClients(this IServiceCollection services, IConfiguration configuration) => 
        services
            .ConfigurarServicoAcessos(configuration)
            .ConfigurarServicoEol(configuration)
            .ConfigurarServicoRelatorio(configuration);
}