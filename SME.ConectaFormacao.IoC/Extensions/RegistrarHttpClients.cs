﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Infra.Servicos.Acessos.IoC;
using SME.ConectaFormacao.Infra.Servicos.Eol.IoC;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.IoC;

namespace SME.ConectaFormacao.IoC.Extensions;

internal static class RegistrarHttpClients
{
    internal static void AdicionarHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        ServicoAcessosCHelper.ConfigurarServicoAcessos(services, configuration);
        ServicoEolCHelper.ConfigurarServicoEol(services, configuration);
        ServicoRelatorioCHelper.ConfigurarServicoRelatorio(services, configuration);
    }
}