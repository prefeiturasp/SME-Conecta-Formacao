using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Autentiacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Aplicacao.Mapeamentos;
using SME.ConectaFormacao.Aplicacao.Pipelines;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Mapeamentos;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
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
        RegistrarValidadoresFluentValidation();
        RegistrarTelemetria();
        RegistrarConexao();
        RegistrarRepositorios();
        RegistrarLogs();
        RegistrarPolly();
        RegistrarMapeamentos();
        RegistrarCasosDeUso();
        RegistrarProfiles();
        RegistrarHttpClients();
    }

    protected virtual void RegistrarProfiles()
    {
        _serviceCollection.AddAutoMapper(typeof(DominioParaDTOProfile), typeof(ServicoParaDTOProfile));
    }

    protected virtual void RegistrarMediatr()
    {
        var assembly = AppDomain.CurrentDomain.Load("SME.ConectaFormacao.Aplicacao");
        _serviceCollection.AddMediatR(x => x.RegisterServicesFromAssemblies(assembly));
    }

    public virtual void RegistrarValidadoresFluentValidation()
    {
        var assembly = AppDomain.CurrentDomain.Load("SME.ConectaFormacao.Aplicacao");

        AssemblyScanner
            .FindValidatorsInAssembly(assembly)
            .ForEach(result => _serviceCollection.AddScoped(result.InterfaceType, result.ValidatorType));

        _serviceCollection.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidacoesPipeline<,>));
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
            config.AddMap(new UsuarioMap());
            config.AddMap(new CriterioValidacaoInscricaoMap());
            config.AddMap(new RoteiroPropostaFormativaMap());
            config.AddMap(new CargoFuncaoMap());

            config.AddMap(new PropostaMap());
            config.AddMap(new PropostaPublicoAlvoMap());
            config.AddMap(new PropostaFuncaoEspecificaMap());
            config.AddMap(new PropostaCriterioValidacaoInscricaoMap());
            config.AddMap(new PropostaVagaRemanecenteMap());

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
        _serviceCollection.TryAddScoped<IRepositorioUsuario, RepositorioUsuario>();
        _serviceCollection.TryAddScoped<IRepositorioCriterioAvaliacaoInscricao, RepositorioCriterioAvaliacaoInscricao>();
        _serviceCollection.TryAddScoped<IRepositorioRoteiroPropostaFormativa, RepositorioRoteiroPropostaFormativa>();
        _serviceCollection.TryAddScoped<IRepositorioCargoFuncao, RepositorioCargoFuncao>();
        _serviceCollection.TryAddScoped<IRepositorioProposta, RepositorioProposta>();
    }

    protected virtual void RegistrarCasosDeUso()
    {
        _serviceCollection.TryAddScoped<ICasoDeUsoAutenticarUsuario, CasoDeUsoAutenticarUsuario>();

        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioMeusDados, CasoDeUsoUsuarioMeusDados>();
        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioAlterarEmail, CasoDeUsoUsuarioAlterarEmail>();
        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioAlterarSenha, CasoDeUsoUsuarioAlterarSenha>();

        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioSolicitarRecuperacaoSenha, CasoDeUsoUsuarioSolicitarRecuperacaoSenha>();
        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioValidarTokenRecuperacaoSenha, CasoDeUsoUsuarioValidarTokenRecuperacaoSenha>();
        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioRecuperarSenha, CasoDeUsoUsuarioRecuperarSenha>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterRoteiroPropostaFormativa, CasoDeUsoObterRoteiroPropostaFormativa>();
    }

    protected virtual void RegistrarHttpClients()
    {
        _serviceCollection.AdicionarHttpClients(_configuration);
    }
}
