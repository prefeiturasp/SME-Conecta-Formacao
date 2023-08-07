﻿using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using SME.ConectaFormacao.Aplicacao.Mapeamentos;
using SME.ConectaFormacao.Aplicacao.Servicos;
using SME.ConectaFormacao.Aplicacao.Servicos.Interface;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Options;
using SME.ConectaFormacao.Infra.Servicos.Polly;
using SME.ConectaFormacao.Infra.Servicos.Telemetria.IoC;
using SME.ConectaFormacao.IoC.Extensions;

namespace SME.ConectaFormacao.IoC;

public class RegistradorDeDependencia
{
    private readonly IServiceCollection _serviceCollection;
    private readonly IConfiguration _configuration;

    public RegistradorDeDependencia(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        _serviceCollection = serviceCollection;
        _configuration = configuration;
    }
    public virtual void Registrar()
    {
        RegistrarMediatr();
        RegistrarTelemetria();
        RegistrarConexao();
        RegistrarRepositorios();
        RegistrarLogs();
        RegistrarPolly();
        RegistrarMapeamentos();
        RegistrarServicos();
        RegistrarProfiles();
        RegistrarHttpClients();
    }
    
    protected virtual void RegistrarProfiles()
    {
        _serviceCollection.AddAutoMapper(typeof(DominioParaDTOProfile));
    }

    protected virtual void RegistrarMediatr()
    {
        var assembly = AppDomain.CurrentDomain.Load("SME.ConectaFormacao.Aplicacao");
        _serviceCollection.AddMediatR(x => x.RegisterServicesFromAssemblies(assembly));
    }
    
    protected virtual void RegistrarLogs()
    {
        _serviceCollection.AddOptions<ConfiguracaoRabbitLogsOptions>()
            .Bind(_configuration.GetSection(ConfiguracaoRabbitLogsOptions.Secao), c => c.BindNonPublicProperties = true);

        _serviceCollection.AddSingleton<ConfiguracaoRabbitLogsOptions>();
        _serviceCollection.AddSingleton<IConexoesRabbitLogs>(serviceProvider =>
        {
            var options = serviceProvider.GetService<IOptions<ConfiguracaoRabbitLogsOptions>>().Value;
            var provider = serviceProvider.GetService<IOptions<DefaultObjectPoolProvider>>().Value;
            return new ConexoesRabbitLogs(options, provider);
        });

        _serviceCollection.AddSingleton<IServicoLogs, ServicoLogs>();
    }

    protected virtual void RegistrarMapeamentos()
    {
        FluentMapper.Initialize(config =>
        {

            config.ForDommel();
        });
    }

    protected virtual void RegistrarTelemetria()
    {
        _serviceCollection.ConfigurarTelemetria(_configuration);
    }

    protected virtual void RegistrarConexao()
    {
        _serviceCollection.AddScoped<IConectaFormacaoConexao, ConectaFormacaoConexao>(_ => new ConectaFormacaoConexao(_configuration.GetConnectionString("conexao")));
        _serviceCollection.AddScoped<ITransacao, Transacao>();
    }

    protected virtual void RegistrarPolly()
    {
        _serviceCollection.ConfigurarPolly();
    }

    protected virtual void RegistrarRepositorios()
    {

    }

    protected virtual void RegistrarServicos()
    {
        _serviceCollection.TryAddScoped<IServicoPerfilUsuario, ServicoPerfilUsuario>();
    }
    protected virtual void RegistrarHttpClients()
    {
        _serviceCollection.AdicionarHttpClients(_configuration);
    }
}
