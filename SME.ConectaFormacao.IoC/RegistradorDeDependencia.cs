using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Ano;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Arquivo;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Autentiacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.ComponenteCurricular;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CriterioCertificacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Email;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Formacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Funcionario;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.FuncionarioExterno.ObterFuncionarioExternoPorCpf;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Grupo;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Modalidade;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Notificacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.PalavraChave;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Ue;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.UsuarioRedeParceria;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ano;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.Arquivo;
using SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular;
using SME.ConectaFormacao.Aplicacao.Interfaces.CriterioCertificacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Email;
using SME.ConectaFormacao.Aplicacao.Interfaces.Formacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Funcionario;
using SME.ConectaFormacao.Aplicacao.Interfaces.FuncionarioExterno.ObterFuncionarioExternoPorCpf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Grupo;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Modalidade;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.PalavraChave;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ue;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Aplicacao.Mapeamentos;
using SME.ConectaFormacao.Aplicacao.Pipelines;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Mapeamentos;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.IoC;
using SME.ConectaFormacao.Infra.Servicos.CacheDistribuido.IoC;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Mensageria.IoC;
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
        ConfigurarMensageria();
        RegistrarConexao();
        RegistrarRepositorios();
        RegistrarLogs();
        RegistrarRabbit();
        RegistrarPolly();
        RegistrarMapeamentos();
        RegistrarCasosDeUso();
        RegistrarProfiles();
        RegistrarHttpClients();
        RegistrarServicoArmazenamento();
        RegistrarCacheDistribuido();
    }

    protected virtual void RegistrarCacheDistribuido()
    {
        _serviceCollection.ConfigurarCacheDistribuidoRedis(_configuration);
    }

    protected virtual void RegistrarServicoArmazenamento()
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
    protected virtual void RegistrarRabbit()
    {
        _serviceCollection.AddOptions<ConfiguracaoRabbitOptions>()
            .Bind(_configuration.GetSection(ConfiguracaoRabbitOptions.Secao), c => c.BindNonPublicProperties = true);

        _serviceCollection.AddSingleton<ConfiguracaoRabbitOptions>();
        _serviceCollection.AddSingleton<IConexoesRabbit>(serviceProvider =>
        {
            var options = serviceProvider.GetService<IOptions<ConfiguracaoRabbitOptions>>().Value;
            var provider = serviceProvider.GetService<IOptions<DefaultObjectPoolProvider>>().Value;
            return new ConexoesRabbitAcessos(options, provider);
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
            config.AddMap(new CargoFuncaoDeparaEolMap());
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
            config.AddMap(new PropostaMovimentacaoMap());
            config.AddMap(new PropostaTurmaMap());
            config.AddMap(new PropostaTurmaDreMap());
            config.AddMap(new PropostaModalidadeMap());
            config.AddMap(new PropostaAnoTurmaMap());
            config.AddMap(new PropostaTurmaVagaMap());
            config.AddMap(new PropostaComponenteCurricularMap());
            config.AddMap(new PropostaDreMap());
            config.AddMap(new PropostaTipoInscricaoMap());
            config.AddMap(new PropostaPareceristaConsideracaoMap());

            config.AddMap(new AreaPromotoraMap());
            config.AddMap(new AreaPromotoraTelefoneMap());

            config.AddMap(new ArquivoMap());

            config.AddMap(new ParametroSistemaMap());
            config.AddMap(new DreMap());

            config.AddMap(new AnoTurmaMap());
            config.AddMap(new ComponenteCurricularMap());
            config.AddMap(new InscricaoMap());

            config.AddMap(new ImportacaoArquivoMap());
            config.AddMap(new ImportacaoArquivoRegistroMap());
            config.AddMap(new PropostaPareceristaMap());
            
            config.AddMap(new NotificacaoMap());
            config.AddMap(new NotificacaoUsuarioMap());

            config.ForDommel();
        });
    }

    protected virtual void RegistrarTelemetria()
    {
        _serviceCollection.ConfigurarTelemetria(_configuration);
    }

    protected virtual void ConfigurarMensageria()
    {
        _serviceCollection.ConfigurarMensageria(_configuration);
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
        _serviceCollection.TryAddScoped<IRepositorioDre, RepositorioDre>();
        _serviceCollection.TryAddScoped<IRepositorioPropostaMovimentacao, RepositorioPropostaMovimentacao>();
        _serviceCollection.TryAddScoped<IRepositorioAnoTurma, RepositorioAnoTurma>();
        _serviceCollection.TryAddScoped<IRepositorioComponenteCurricular, RepositorioComponenteCurricular>();
        _serviceCollection.TryAddScoped<IRepositorioCargoFuncaoDeparaEol, RepositorioCargoFuncaoDeparaEol>();
        _serviceCollection.TryAddScoped<IRepositorioInscricao, RepositorioInscricao>();
        _serviceCollection.TryAddScoped<IRepositorioImportacaoArquivo, RepositorioImportacaoArquivo>();
        _serviceCollection.TryAddScoped<IRepositorioImportacaoArquivoRegistro, RepositorioImportacaoArquivoRegistro>();
        _serviceCollection.TryAddScoped<IRepositorioPropostaPareceristaConsideracao, RepositorioPropostaPareceristaConsideracao>();
        
        _serviceCollection.TryAddScoped<IRepositorioNotificacao, RepositorioNotificacao>();
        _serviceCollection.TryAddScoped<IRepositorioNotificacaoUsuario, RepositorioNotificacaoUsuario>();
    }

    protected virtual void RegistrarCasosDeUso()
    {
        _serviceCollection.TryAddScoped<ICasoDeUsoAutenticarUsuario, CasoDeUsoAutenticarUsuario>();
        _serviceCollection.TryAddScoped<ICasoDeUsoAutenticarRevalidar, CasoDeUsoAutenticarRevalidar>();
        _serviceCollection.TryAddScoped<ICasoDeUsoAutenticarAlterarPerfil, CasoDeUsoAutenticarAlterarPerfil>();

        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioMeusDados, CasoDeUsoUsuarioMeusDados>();
        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioAlterarEmail, CasoDeUsoUsuarioAlterarEmail>();
        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioAlterarEmailEducacional, CasoDeUsoUsuarioAlterarEmailEducacional>();
        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioAlterarSenha, CasoDeUsoUsuarioAlterarSenha>();
        _serviceCollection.TryAddScoped<ICasoDeUsoInserirUsuarioExterno, CasoDeUsoInserirUsuarioExterno>();
        _serviceCollection.TryAddScoped<ICasoDeUsoReenviarEmail, CasoDeUsoReenviarEmail>();
        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioAlterarNome, CasoDeUsoUsuarioAlterarNome>();
        _serviceCollection.TryAddScoped<ICasoDeUsoAlterarEmailEReenviarEmailParaValidacao, CasoDeUsoAlterarEmailEReenviarEmailParaValidacao>();

        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioSolicitarRecuperacaoSenha, CasoDeUsoUsuarioSolicitarRecuperacaoSenha>();
        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioValidacaoSenhaToken, CasoDeUsoUsuarioValidacaoSenhaToken>();
        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioValidacaoEmailToken, CasoDeUsoUsuarioValidacaoEmailToken>();
        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioRecuperarSenha, CasoDeUsoUsuarioRecuperarSenha>();
        _serviceCollection.TryAddScoped<ICasoDeUsoUsuarioAlterarUnidadeEol, CasoDeUsoUsuarioAlterarUnidadeEol>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterRoteiroPropostaFormativa, CasoDeUsoObterRoteiroPropostaFormativa>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterCargoFuncao, CasoDeUsoObterCargoFuncao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterCriterioValidacaoInscricao, CasoDeUsoObterCriterioValidacaoInscricao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterGrupoSistema, CasoDeUsoObterGrupoSistema>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterPalavraChave, CasoDeUsoObterPalavraChave>();
        _serviceCollection.TryAddScoped<ICasoDeUsoCriterioCertificacao, CasoDeUsoCriterioCertificacao>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterTiposAreaPromotora, CasoDeUsoObterTiposAreaPromotora>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterAreaPromotoraPaginada, CasoDeUsoObterAreaPromotoraPaginada>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterAreaPromotoraPorId, CasoDeUsoObterAreaPromotoraPorId>();
        _serviceCollection.TryAddScoped<ICasoDeUsoInserirAreaPromotora, CasoDeUsoInserirAreaPromotora>();
        _serviceCollection.TryAddScoped<ICasoDeUsoAlterarAreaPromotora, CasoDeUsoAlterarAreaPromotora>();
        _serviceCollection.TryAddScoped<ICasoDeUsoRemoverAreaPromotora, CasoDeUsoRemoverAreaPromotora>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterAreaPromotoraLista, CasoDeUsoObterAreaPromotoraLista>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterAreaPromotoraListaAreaPublica, CasoDeUsoObterAreaPromotoraListaAreaPublica>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterAreaPromotoraListaRedeParceria, CasoDeUsoObterAreaPromotoraListaRedeParceria>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterTipoFormacao, CasoDeUsoObterTipoFormacao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterTipoInscricao, CasoDeUsoObterTipoInscricao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterFormatos, CasoDeUsoObterFormatos>();
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
        _serviceCollection.TryAddScoped<ICasoDeUsoObterPropostaTutorPorId, CasoDeUsoObterPropostaTutorPorId>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterListaDre, CasoDeUsoObterListaDre>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterUnidadePorCodigoEol, CasoDeUsoObterUnidadePorCodigoEol>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterFuncionarioExternoPorCpf, CasoDeUsoObterFuncionarioExternoPorCpf>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterPropostasDashboard, CasoDeUsoObterPropostasDashboard>();
        _serviceCollection.TryAddScoped<ICasoDeUsoSalvarPropostaPareceristaConsideracao, CasoDeUsoSalvarPropostaPareceristaConsideracao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterPropostaParecer, CasoDeUsoObterPropostaParecer>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterRelatorioPropostaLaudaPublicacao, CasoDeUsoObterRelatorioPropostaLaudaPublicacao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterSugestaoParecerPareceristas, CasoDeUsoObterSugestaoParecerPareceristas>();
        _serviceCollection.TryAddScoped<ICasoDeUsoAprovarProposta, CasoDeUsoAprovarProposta>();
        _serviceCollection.TryAddScoped<ICasoDeUsoRecusarProposta, CasoDeUsoRecusarProposta>();
      
        _serviceCollection.TryAddScoped<ICasoDeUsoObterRelatorioPropostaLaudaPublicacao, CasoDeUsoObterRelatorioPropostaLaudaPublicacao>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterRelatorioPropostaLaudaCompleta, CasoDeUsoObterRelatorioPropostaLaudaCompleta>();

        _serviceCollection.TryAddScoped<ICasoDeUsoSalvarPropostaEncontro, CasoDeUsoSalvarPropostaEncontro>();
        _serviceCollection.TryAddScoped<ICasoDeUsoRemoverPropostaEncontro, CasoDeUsoRemoverPropostaEncontro>();

        _serviceCollection.TryAddScoped<ICasoDeUsoArquivoCarregarTemporario, CasoDeUsoArquivoCarregarTemporario>();
        _serviceCollection.TryAddScoped<ICasoDeUsoArquivoExcluir, CasoDeUsoArquivoExcluir>();
        _serviceCollection.TryAddScoped<ICasoDeUsoArquivoBaixar, CasoDeUsoArquivoBaixar>();

        _serviceCollection.TryAddScoped<IExecutarSincronizacaoInstitucionalDreSyncUseCase, ExecutarSincronizacaoInstitucionalDreSyncUseCase>();
        _serviceCollection.TryAddScoped<IExecutarSincronizacaoInstitucionalDreTratarUseCase, ExecutarSincronizacaoInstitucionalDreTratarUseCase>();
        _serviceCollection.TryAddScoped<ICasoDeUsoEnviarProposta, CasoDeUsoEnviarProposta>();

        _serviceCollection.TryAddScoped<ICasoDeUsoDevolverProposta, CasoDeUsoDevolverProposta>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterFormacaoHomologada, CasoDeUsoObterFormacaoHomologada>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterListaComponentesCurriculares, CasoDeUsoObterListaComponentesCurriculares>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterListaAnoTurma, CasoDeUsoObterListaAnoTurma>();

        _serviceCollection.TryAddScoped<IExecutarSincronizacaoComponentesCurricularesEAnosTurmaEOLUseCase, ExecutarSincronizacaoComponentesCurricularesEAnosTurmaEolUseCase>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterModalidade, CasoDeUsoObterModalidade>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterTodosFormatos, CasoDeUsoObterTodosFormatos>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterListagemFormacaoPaginada, CasoDeUsoObterListagemFormacaoPaginada>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterFormacaoDetalhada, CasoDeUsoObterFormacaoDetalhada>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterDadosInscricao, CasoDeUsoObterDadosInscricao>();

        _serviceCollection.TryAddScoped<ICasoDeUsoGerarPropostaTurmaVaga, CasoDeUsoGerarPropostaTurmaVaga>();
        _serviceCollection.TryAddScoped<ICasoDeUsoSalvarInscricao, CasoDeUsoSalvarInscricao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoCancelarInscricao, CasoDeUsoCancelarInscricao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterInscricaoPorId, CasoDeUsoObterInscricaoPorId>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterTurmasInscricao, CasoDeUsoObterTurmasInscricao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterInscricaoPaginada, CasoDeUsoObterInscricaoPaginada>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterDadosPaginadosComFiltros, CasoDeUsoObterDadosPaginadosComFiltros>();
        _serviceCollection.TryAddScoped<ICasoDeUsoAlterarVinculoInscricao, CasoDeUsoAlterarVinculoInscricao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterInformacoesInscricoesEstaoAbertasPorId, CasoDeUsoObterInformacoesInscricoesEstaoAbertasPorId>();

        _serviceCollection.TryAddScoped<ICasoDeUsoRealizarInscricaoAutomatica, CasoDeUsoRealizarInscricaoAutomatica>();
        _serviceCollection.TryAddScoped<ICasoDeUsoRealizarInscricaoAutomaticaTratarTurmas, CasoDeUsoRealizarInscricaoAutomaticaTratarTurmas>();
        _serviceCollection.TryAddScoped<ICasoDeUsoRealizarInscricaoAutomaticaTratarCursista, CasoDeUsoRealizarInscricaoAutomaticaTratarCursista>();
        _serviceCollection.TryAddScoped<ICasoDeUsoRealizarInscricaoAutomaticaInscreverCursista, CasoDeUsoRealizarInscricaoAutomaticaInscreverCursista>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterInscricaoTipo, CasoDeUsoObterInscricaoTipo>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterNomeCpfCursistaInscricao, CasoDeUsoObterNomeCpfCpfCursistaInscricao>();

        _serviceCollection.TryAddScoped<ICasoDeUsoImportacaoArquivoInscricaoCursista, CasoDeUsoImportacaoInscricaoCursista>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterArquivosInscricaoImportados, CasoDeUsoObterArquivosInscricaoImportados>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterRegistrosDaIncricaoInconsistentes, CasoDeUsoObterRegistrosDaIncricaoInconsistentes>();
        _serviceCollection.TryAddScoped<ICasoDeUsoInscricaoManualContinuarProcessamento, CasoDeUsoInscricaoManualContinuarProcessamento>();
        _serviceCollection.TryAddScoped<ICasoDeUsoInscricaoManualCancelarProcessamento, CasoDeUsoInscricaoManualCancelarProcessamento>();
        _serviceCollection.TryAddScoped<ICasoDeUsoProcessarArquivoDeImportacaoInscricao, CasoDeUsoProcessarArquivoDeImportacaoInscricao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao, CasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao>();

        _serviceCollection.TryAddScoped<ICasoDeUsoSalvarInscricaoManual, CasoDeUsoSalvarInscricaoManual>();

        _serviceCollection.TryAddScoped<ICasoDeUsoImportacaoInscricaoCursistaValidar, CasoDeUsoImportacaoInscricaoCursistaValidar>();
        _serviceCollection.TryAddScoped<ICasoDeUsoImportacaoInscricaoCursistaValidarItem, CasoDeUsoImportacaoInscricaoCursistaValidarItem>();

        _serviceCollection.TryAddScoped<ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista, CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista>();
        _serviceCollection.TryAddScoped<ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar, CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterTiposEmail, CasoDeUsoObterTiposEmail>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterUsuariosAdminDf, CasoDeUsoObterUsuariosAdminDf>();

        _serviceCollection.TryAddScoped<ICasoDeUsoEnviarEmailDevolverProposta, CasoDeUsoEnviarEmailDevolverProposta>();
        _serviceCollection.TryAddScoped<ICasoDeUsoEncerrarInscricaoCursistaInativoSemCargo, CasoDeUsoEncerrarInscricaoCursistaInativoSemCargo>();
        _serviceCollection.TryAddScoped<ICasoDeUsoEncerrarInscricaoAutomaticamenteTurma, CasoDeUsoEncerrarInscricaoAutomaticamenteTurma>();
        _serviceCollection.TryAddScoped<ICasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes, CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes>();
        _serviceCollection.TryAddScoped<ICasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios, CasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterParecerista, CasoDeUsoObterParecerista>();
        _serviceCollection.TryAddScoped<ICasoDeUsoRemoverParecerDaProposta, CasoDeUsoRemoverParecerDaProposta>();
        _serviceCollection.TryAddScoped<ICasoDeUsoEnviarPropostaParecerista, CasoDeUsoEnviarPropostaParecerista>();
        _serviceCollection.TryAddScoped<ICasoDeUsoAprovarPropostaParecerista, CasoDeUsoAprovarPropostaParecerista>();
        _serviceCollection.TryAddScoped<ICasoDeUsoRecusarPropostaParecerista, CasoDeUsoRecusarPropostaParecerista>();       
        _serviceCollection.TryAddScoped<ICasoDeUsoObterHorasTotaisProposta, CasoDeUsoObterHorasTotaisProposta>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterSituacaoUsuarioRedeParceria, CasoDeUsoObterSituacaoUsuarioRedeParceria>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterUsuarioRedeParceriaPaginada, CasoDeUsoObterUsuarioRedeParceriaPaginada>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterUsuarioRedeParceriaPorId, CasoDeUsoObterUsuarioRedeParceriaPorId>();
        _serviceCollection.TryAddScoped<ICasoDeUsoInserirUsuarioRedeParceria, CasoDeUsoInserirUsuarioRedeParceria>();
        _serviceCollection.TryAddScoped<ICasoDeUsoAlterarUsuarioRedeParceria, CasoDeUsoAlterarUsuarioRedeParceria>();
        _serviceCollection.TryAddScoped<ICasoDeUsoRemoverUsuarioRedeParceria, CasoDeUsoRemoverUsuarioRedeParceria>();

        _serviceCollection.TryAddScoped<ICasoDeUsoObterTotalNotificacaoNaoLida, CasoDeUsoObterTotalNotificacaoNaoLida>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterNotificacao, CasoDeUsoObterNotificacao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterNotificacaoPaginada, CasoDeUsoObterNotificacaoPaginada>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterCategoriaNotificacao, CasoDeUsoObterCategoriaNotificacao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterTipoNotificacao, CasoDeUsoObterTipoNotificacao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoObterSituacaoNotificacao, CasoDeUsoObterSituacaoNotificacao>();

        _serviceCollection.TryAddScoped<ICasoDeUsoEnviarEmail, CasoDeUsoEnviarEmail>();
        _serviceCollection.TryAddScoped<ICasoDeUsoEnviarNotificacao, CasoDeUsoEnviarNotificacao>();
        _serviceCollection.TryAddScoped<ICasoDeUsoNotificarPareceristasSobreAtribuicaoPelaDF, CasoDeUsoNotificarPareceristasSobreAtribuicaoPelaDF>();
        _serviceCollection.TryAddScoped<ICasoDeUsoNotificarDFPeloEnvioParecerPeloParecerista, CasoDeUsoNotificarDFPeloEnvioParecerPeloParecerista>();
        _serviceCollection.TryAddScoped<ICasoDeUsoNotificarAreaPromotoraParaAnaliseParecer, CasoDeUsoNotificarAreaPromotoraParaAnaliseParecer>();
        _serviceCollection.TryAddScoped<ICasoDeUsoNotificarPareceristasParaReanalise, CasoDeUsoNotificarPareceristasParaReanalise>();
        _serviceCollection.TryAddScoped<ICasoDeUsoNotificarResponsavelDFSobreReanaliseDoParecerista, CasoDeUsoNotificarResponsavelDFSobreReanaliseDoParecerista>();
        _serviceCollection.TryAddScoped<ICasoDeUsoNotificarAreaPromotoraSobreValidacaoFinalPelaDF, CasoDeUsoNotificarAreaPromotoraSobreValidacaoFinalPelaDF>();
    }

    protected virtual void RegistrarHttpClients()
    {
        _serviceCollection.AdicionarHttpClients(_configuration);
    }
}