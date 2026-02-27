using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Infra.Servicos.Compactacao.Interfaces;

namespace SME.ConectaFormacao.Infra.Servicos.Compactacao.Ioc
{
    public static class ServicoCompactacaoCHelper
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection ConfigurarServicoCompactacao() =>
                services.AddSingleton<IServicoCompactacao, ServicoCompactacao>();
        }
    }
}
