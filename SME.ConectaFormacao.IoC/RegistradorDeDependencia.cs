﻿using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Arquivo;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Autentiacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CriterioCertificacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Grupo;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.PalavraChave;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.Arquivo;
using SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Interfaces.CriterioCertificacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Grupo;
using SME.ConectaFormacao.Aplicacao.Interfaces.PalavraChave;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Aplicacao.Mapeamentos;
using SME.ConectaFormacao.Aplicacao.Pipelines;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Mapeamentos;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.IoC;
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
        RegistrarServicoArmazenamento();
    }

    private void RegistrarServicoArmazenamento()
    {
        _serviceCollection.ConfigurarArmazenamento(_configuration);
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
            config.AddMap(new PalavraChaveMap());
            config.AddMap(new CriterioCertificacaoMap());
            
            config.AddMap(new PropostaMap());
            config.AddMap(new PropostaPublicoAlvoMap());
            config.AddMap(new PropostaFuncaoEspecificaMap());
            config.AddMap(new PropostaCriterioValidacaoInscricaoMap());
            config.AddMap(new PropostaVagaRemanecenteMap());
            config.AddMap(new PropostaEncontroMap());
            config.AddMap(new PropostaEncontroDataMap());
            config.AddMap(new PropostaCriterioCertificacaoMap());
            config.AddMap(new PropostaEncontroTurmaMap());
            config.AddMap(new PropostaPalavraChaveMap());
            config.AddMap(new PropostaRegenteTurmaMap());
            config.AddMap(new PropostaRegenteMap());
            config.AddMap(new PropostaTutorTurmaMap());
            config.AddMap(new PropostaTutorMap());
            
            config.AddMap(new AreaPromotoraMap());
            config.AddMap(new AreaPromotoraTelefoneMap());

            config.AddMap(new ArquivoMap());
            
            config.AddMap(new ParametroSistemaMap());

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
        _serviceCollection.TryAddScoped<IRepositorioCriterioValidacaoInscricao, RepositorioCriterioValidacaoInscricao>();
        _serviceCollection.TryAddScoped<IRepositorioRoteiroPropostaFormativa, RepositorioRoteiroPropostaFormativa>();
        _serviceCollection.TryAddScoped<IRepositorioCargoFuncao, RepositorioCargoFuncao>();
        _serviceCollection.TryAddScoped<IRepositorioProposta, RepositorioProposta>();
        _serviceCollection.TryAddScoped<IRepositorioAreaPromotora, RepositorioAreaPromotora>();
        _serviceCollection.TryAddScoped<IRepositorioArquivo, RepositorioArquivo>();
        _serviceCollection.TryAddScoped<IRepositorioPalavraChave, RepositorioPalavraChave>();
        _serviceCollection.TryAddScoped<IRepositorioCriterioCertificacao, RepositorioCriterioCertificacao>();
        _serviceCollection.TryAddScoped<IRepositorioParametroSistema, RepositorioParametroSistema>();
        _serviceCollection.TryAddScoped<IRepositorioPropostaTutor, RepositorioPropostaTutor>();
        _serviceCollection.TryAddScoped<IRepositorioPropostaRegente, RepositorioPropostaRegente>();
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
        _serviceCollection.TryAddScoped<ICasoDeUsoObterCargoFuncao, CasoDeUsoObterCargoFuncao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterCriterioValidacaoInscricao, CasoDeUsoObterCriterioValidacaoInscricao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterGrupos, CasoDeUsoObterGrupos>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterPalavraChave, CasoDeUsoObterPalavraChave>();
        _serviceCollection.TryAddScoped<ICasoDeUsoCriterioCertificacao, CasoDeUsoCriterioCertificacao>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterTiposAreaPromotora, CasoDeUsoObterTiposAreaPromotora>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterAreaPromotoraPaginada, CasoDeUsoObterAreaPromotoraPaginada>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterAreaPromotoraPorId, CasoDeUsoObterAreaPromotoraPorId>();
        _serviceCollection.TryAddScoped<ICasoDeUsoInserirAreaPromotora, CasoDeUsoInserirAreaPromotora>();
        _serviceCollection.TryAddScoped<ICasoDeUsoAlterarAreaPromotora, CasoDeUsoAlterarAreaPromotora>();
        _serviceCollection.TryAddScoped<ICasoDeUsoRemoverAreaPromotora, CasoDeUsoRemoverAreaPromotora>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterAreaPromotoraLista, CasoDeUsoObterAreaPromotoraLista>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterTipoFormacao, CasoDeUsoObterTipoFormacao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterTipoInscricao, CasoDeUsoObterTipoInscricao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterModalidades, CasoDeUsoObterModalidades>();
        _serviceCollection.TryAddScoped<ICasoDeUsoInserirProposta, CasoDeUsoInserirProposta>();
        _serviceCollection.TryAddScoped<ICasoDeUsoAlterarProposta, CasoDeUsoAlterarProposta>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterPropostaPorId, CasoDeUsoObterPropostaPorId>();
        _serviceCollection.TryAddScoped<ICasoDeUsoRemoverProposta, CasoDeUsoRemoverProposta>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterSituacoesProposta, CasoDeUsoObterSituacoesProposta>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterPropostaPaginacao, CasoDeUsoObterPropostaPaginacao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterInformacoesCadastrante, CasoDeUsoObterInformacoesCadastrante>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterTurmasProposta, CasoDeUsoObterTurmasProposta>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterTipoEncontro, CasoDeUsoObterTipoEncontro>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterPropostaEncontroPaginacao, CasoDeUsoObterPropostaEncontroPaginacao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterComunicadoAcaoFormativa, CasoDeUsoObterComunicadoComunicadoAcaoFormativa>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterNomeRegenteTutor, CasoDeUsoObterNomeRegenteTutor>();
        _serviceCollection.TryAddScoped<ICasoDeUsoSalvarPropostaRegente, CasoDeUsoSalvarPropostaRegente>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterPropostaRegentePaginacao, CasoDeUsoObterPropostaRegentePaginacao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterPropostaRegentePorId, CasoDeUsoObterPropostaRegentePorId>();
        _serviceCollection.TryAddScoped<ICasoDeUsoRemoverPropostaRegente, CasoDeUsoRemoverPropostaRegente>();
        _serviceCollection.TryAddScoped<ICasoDeUsoSalvarPropostaTutor, CasoDeUsoSalvarPropostaTutor>();
        _serviceCollection.TryAddScoped<ICasoDeUsoRemoverPropostaTutor, CasoDeUsoRemoverPropostaTutor>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterPropostaTutorPaginacao, CasoDeUsoObterPropostaTutorPaginacao>();



        _serviceCollection.TryAddScoped<ICasoDeUsoSalvarPropostaEncontro, CasoDeUsoSalvarPropostaEncontro>();
        _serviceCollection.TryAddScoped<ICasoDeUsoRemoverPropostaEncontro, CasoDeUsoRemoverPropostaEncontro>();

        _serviceCollection.TryAddScoped<ICasoDeUsoArquivoCarregarTemporario, CasoDeUsoArquivoCarregarTemporario>();
        _serviceCollection.TryAddScoped<ICasoDeUsoArquivoExcluir, CasoDeUsoArquivoExcluir>();
        _serviceCollection.TryAddScoped<ICasoDeUsoArquivoBaixar, CasoDeUsoArquivoBaixar>();

    }

    protected virtual void RegistrarHttpClients()
    {
        _serviceCollection.AdicionarHttpClients(_configuration);
    }
}
