using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces;

namespace SME.ConectaFormacao.Infra.Servicos.Emails.IoC
{
    public static class ServicoEmailsCHelper
    {
        public static IServiceCollection ConfigurarServicoEmails(this IServiceCollection services) =>
            services
            .AddSingleton<ISmtpClientFactory, SmtpClientFactory>()
            .AddSingleton<IServicoEnvioEmail, ServicoEnvioEmail>();
    }
}