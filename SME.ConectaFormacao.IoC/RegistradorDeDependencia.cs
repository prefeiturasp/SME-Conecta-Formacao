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
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Modalidade;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Notificacoes;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.PalavraChave;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.SincronizacaoEOL;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Ue;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.UsuariosRedeParceria;
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
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Modalidade;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.PalavraChave;
using SME.ConectaFormacao.Aplicacao.Interfaces.SincronizacaoEOL;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ue;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Aplicacao.Mapeamentos;
using SME.ConectaFormacao.Aplicacao.Pipelines;
using SME.ConectaFormacao.Dominio.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Mapeamentos;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Templates;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.IoC;
using SME.ConectaFormacao.Infra.Servicos.CacheDistribuido.IoC;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Mensageria.IoC;
using SME.ConectaFormacao.Infra.Servicos.Notificacao;
using SME.ConectaFormacao.Infra.Servicos.Options;
using SME.ConectaFormacao.Infra.Servicos.Polly;
using SME.ConectaFormacao.Infra.Servicos.Telemetria.IoC;
using SME.ConectaFormacao.IoC.Extensions;
using SME.ConectaFormacao.IoC.Features;

namespace SME.ConectaFormacao.IoC;

public class RegistradorDeDependencia(IServiceCollection serviceCollection, IConfiguration configuration)
{
    public virtual void Registrar()
    {
        RegistrarMediatr();
        RegistrarValidadoresFluentValidation();
        RegistrarTelemetria();
        ConfigurarMensageria();
        RegistrarConexao();
        RegistrarRepositorios();
        RegistrarServices();
        RegistrarLogs();
        RegistrarRabbit();
        RegistrarPolly();
        RegistrarMapeamentos();
        RegistrarCasosDeUso();
        RegistrarProfiles();
        RegistrarHttpClients();
        RegistrarServicoArmazenamento();
        RegistrarCacheDistribuido();
        serviceCollection
            .AddSingleton<ITemplateService, TemplateService>()
            .AdicionarModuloCodaf()
            .AdicionarModuloProposta()
            ;
    }

    protected virtual void RegistrarCacheDistribuido()
    {
        serviceCollection.ConfigurarCacheDistribuidoRedis(configuration);
    }

    protected virtual void RegistrarServicoArmazenamento()
    {
        serviceCollection.ConfigurarArmazenamento(configuration);
    }

    protected virtual void RegistrarProfiles()
    {
        serviceCollection.AddAutoMapper(cfg => cfg.AddMaps(typeof(AssemblyProfile).Assembly));
    }

    protected virtual void RegistrarMediatr()
    {
        var assembly = AppDomain.CurrentDomain.Load("SME.ConectaFormacao.Aplicacao");
        serviceCollection.AddMediatR(x => x.RegisterServicesFromAssemblies(assembly));
    }

    public virtual void RegistrarValidadoresFluentValidation()
    {
        var assembly = AppDomain.CurrentDomain.Load("SME.ConectaFormacao.Aplicacao");

        AssemblyScanner
            .FindValidatorsInAssembly(assembly)
            .ForEach(result => serviceCollection.AddScoped(result.InterfaceType, result.ValidatorType));

        serviceCollection.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidacoesPipeline<,>));
    }

    protected virtual void RegistrarLogs()
    {
        serviceCollection.AddOptions<ConfiguracaoRabbitLogsOptions>()
            .Bind(configuration.GetSection(ConfiguracaoRabbitLogsOptions.Secao), c => c.BindNonPublicProperties = true);

        serviceCollection.AddSingleton<ConfiguracaoRabbitLogsOptions>();
        serviceCollection.AddSingleton<IConexoesRabbitLogs>(serviceProvider =>
        {
            var options = serviceProvider.GetService<IOptions<ConfiguracaoRabbitLogsOptions>>().Value;
            var provider = serviceProvider.GetService<IOptions<DefaultObjectPoolProvider>>().Value;
            return new ConexoesRabbitLogs(options, provider);
        });

        serviceCollection.AddSingleton<IServicoLogs, ServicoLogs>();
    }
    protected virtual void RegistrarRabbit()
    {
        serviceCollection.AddOptions<ConfiguracaoRabbitOptions>()
            .Bind(configuration.GetSection(ConfiguracaoRabbitOptions.Secao), c => c.BindNonPublicProperties = true);

        serviceCollection.AddSingleton<ConfiguracaoRabbitOptions>();
        serviceCollection.AddSingleton<IConexoesRabbit>(serviceProvider =>
        {
            var options = serviceProvider.GetService<IOptions<ConfiguracaoRabbitOptions>>().Value;
            var provider = serviceProvider.GetService<IOptions<DefaultObjectPoolProvider>>().Value;
            return new ConexoesRabbitAcessos(options, provider);
        });

        serviceCollection.AddSingleton<IServicoLogs, ServicoLogs>();
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

            config.AddMap(new CodafComentarioListaPresencaMap());
            config.AddMap(new CodafInscricaoMap());
            config.AddMap(new CodafRetificacaoMap());
            config.AddMap(new CodafListaPresencaMap());
            config.AddMap(new CodafMovimentacaoListaPresencaMap());
            config.AddMap(new CodafCertificadoMap());

            config.AddMap(new CodafAnexoMap());

            config.ForDommel();
        });
    }

    protected virtual void RegistrarTelemetria()
    {
        serviceCollection.ConfigurarTelemetria(configuration);
    }

    protected virtual void ConfigurarMensageria()
    {
        serviceCollection.ConfigurarMensageria(configuration);
    }

    protected virtual void RegistrarConexao()
    {
        serviceCollection.AddScoped<IConectaFormacaoConexao, ConectaFormacaoConexao>(_ => new ConectaFormacaoConexao(configuration.GetConnectionString("conexao")));
        serviceCollection.AddScoped<ITransacao, Transacao>();
    }

    protected virtual void RegistrarPolly()
    {
        serviceCollection.ConfigurarPolly();
    }

    protected virtual void RegistrarRepositorios()
    {
        serviceCollection.TryAddScoped<IRepositorioUsuario, RepositorioUsuario>();
        serviceCollection.TryAddScoped<IRepositorioCriterioValidacaoInscricao, RepositorioCriterioValidacaoInscricao>();
        serviceCollection.TryAddScoped<IRepositorioRoteiroPropostaFormativa, RepositorioRoteiroPropostaFormativa>();
        serviceCollection.TryAddScoped<IRepositorioCargoFuncao, RepositorioCargoFuncao>();        
        serviceCollection.TryAddScoped<IRepositorioAreaPromotora, RepositorioAreaPromotora>();
        serviceCollection.TryAddScoped<IRepositorioArquivo, RepositorioArquivo>();
        serviceCollection.TryAddScoped<IRepositorioPalavraChave, RepositorioPalavraChave>();
        serviceCollection.TryAddScoped<IRepositorioCriterioCertificacao, RepositorioCriterioCertificacao>();
        serviceCollection.TryAddScoped<IRepositorioParametroSistema, RepositorioParametroSistema>();                
        serviceCollection.TryAddScoped<IRepositorioDre, RepositorioDre>();        
        serviceCollection.TryAddScoped<IRepositorioAnoTurma, RepositorioAnoTurma>();
        serviceCollection.TryAddScoped<IRepositorioComponenteCurricular, RepositorioComponenteCurricular>();
        serviceCollection.TryAddScoped<IRepositorioCargoFuncaoDeparaEol, RepositorioCargoFuncaoDeparaEol>();
        serviceCollection.TryAddScoped<IRepositorioInscricao, RepositorioInscricao>();
        serviceCollection.TryAddScoped<IRepositorioImportacaoArquivo, RepositorioImportacaoArquivo>();
        serviceCollection.TryAddScoped<IRepositorioImportacaoArquivoRegistro, RepositorioImportacaoArquivoRegistro>();        
        serviceCollection.TryAddScoped<IRepositorioNotificacao, RepositorioNotificacao>();
        serviceCollection.TryAddScoped<IRepositorioNotificacaoUsuario, RepositorioNotificacaoUsuario>();
        serviceCollection.AddScoped<IRepositorioCargoEol, RepositorioCargoEol>();
        serviceCollection.AddScoped<IRepositorioSincronizador, RepositorioSincronizador>();
        serviceCollection.AddScoped<IRepositorioAtribuicaoAulaServidor, RepositorioAtribuicaoAulaServidor>();
        serviceCollection.AddScoped<IRepositorioFuncaoAtividadeUsuario, RepositorioFuncaoAtividadeUsuario>();
        serviceCollection.AddScoped<IRepositorioCargoFuncaoEol, RepositorioCargoFuncaoEol>();
        serviceCollection.AddScoped<IRepositorioCodafListaPresenca, RepositorioCodafListaPresenca>();
    }

    protected virtual void RegistrarCasosDeUso()
    {
        serviceCollection.TryAddScoped<ICasoDeUsoAutenticarUsuario, CasoDeUsoAutenticarUsuario>();
        serviceCollection.TryAddScoped<ICasoDeUsoAutenticarRevalidar, CasoDeUsoAutenticarRevalidar>();
        serviceCollection.TryAddScoped<ICasoDeUsoAutenticarAlterarPerfil, CasoDeUsoAutenticarAlterarPerfil>();

        serviceCollection.TryAddScoped<ICasoDeUsoUsuarioMeusDados, CasoDeUsoUsuarioMeusDados>();
        serviceCollection.TryAddScoped<ICasoDeUsoUsuarioAlterarEmail, CasoDeUsoUsuarioAlterarEmail>();
        serviceCollection.TryAddScoped<ICasoDeUsoUsuarioAlterarEmailEducacional, CasoDeUsoUsuarioAlterarEmailEducacional>();
        serviceCollection.TryAddScoped<ICasoDeUsoUsuarioAlterarSenha, CasoDeUsoUsuarioAlterarSenha>();
        serviceCollection.TryAddScoped<ICasoDeUsoInserirUsuarioExterno, CasoDeUsoInserirUsuarioExterno>();
        serviceCollection.TryAddScoped<ICasoDeUsoReenviarEmail, CasoDeUsoReenviarEmail>();
        serviceCollection.TryAddScoped<ICasoDeUsoUsuarioAlterarNome, CasoDeUsoUsuarioAlterarNome>();
        serviceCollection.TryAddScoped<ICasoDeUsoAlterarEmailEReenviarEmailParaValidacao, CasoDeUsoAlterarEmailEReenviarEmailParaValidacao>();
        serviceCollection.TryAddScoped<ICasoDeUsoUsuarioAlterarTipoEmail, CasoDeUsoUsuarioAlterarTipoEmail>();

        serviceCollection.TryAddScoped<ICasoDeUsoUsuarioSolicitarRecuperacaoSenha, CasoDeUsoUsuarioSolicitarRecuperacaoSenha>();
        serviceCollection.TryAddScoped<ICasoDeUsoUsuarioValidacaoSenhaToken, CasoDeUsoUsuarioValidacaoSenhaToken>();
        serviceCollection.TryAddScoped<ICasoDeUsoUsuarioValidacaoEmailToken, CasoDeUsoUsuarioValidacaoEmailToken>();
        serviceCollection.TryAddScoped<ICasoDeUsoUsuarioRecuperarSenha, CasoDeUsoUsuarioRecuperarSenha>();
        serviceCollection.TryAddScoped<ICasoDeUsoUsuarioAlterarUnidadeEol, CasoDeUsoUsuarioAlterarUnidadeEol>();

        serviceCollection.TryAddScoped<ICasoDeUsoObterCargoFuncao, CasoDeUsoObterCargoFuncao>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterGrupoSistema, CasoDeUsoObterGrupoSistema>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterPalavraChave, CasoDeUsoObterPalavraChave>();
        serviceCollection.TryAddScoped<ICasoDeUsoCriterioCertificacao, CasoDeUsoCriterioCertificacao>();

        serviceCollection.TryAddScoped<ICasoDeUsoObterTiposAreaPromotora, CasoDeUsoObterTiposAreaPromotora>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterAreaPromotoraPaginada, CasoDeUsoObterAreaPromotoraPaginada>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterAreaPromotoraPorId, CasoDeUsoObterAreaPromotoraPorId>();
        serviceCollection.TryAddScoped<ICasoDeUsoInserirAreaPromotora, CasoDeUsoInserirAreaPromotora>();
        serviceCollection.TryAddScoped<ICasoDeUsoAlterarAreaPromotora, CasoDeUsoAlterarAreaPromotora>();
        serviceCollection.TryAddScoped<ICasoDeUsoRemoverAreaPromotora, CasoDeUsoRemoverAreaPromotora>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterAreaPromotoraLista, CasoDeUsoObterAreaPromotoraLista>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterAreaPromotoraListaAreaPublica, CasoDeUsoObterAreaPromotoraListaAreaPublica>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterAreaPromotoraListaRedeParceria, CasoDeUsoObterAreaPromotoraListaRedeParceria>();

        serviceCollection.TryAddScoped<ICasoDeUsoObterListaDre, CasoDeUsoObterListaDre>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterUnidadePorCodigoEol, CasoDeUsoObterUnidadePorCodigoEol>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterFuncionarioExternoPorCpf, CasoDeUsoObterFuncionarioExternoPorCpf>();                                                        
        
        
                

        serviceCollection.TryAddScoped<ICasoDeUsoArquivoCarregarTemporario, CasoDeUsoArquivoCarregarTemporario>();
        serviceCollection.TryAddScoped<ICasoDeUsoArquivoExcluir, CasoDeUsoArquivoExcluir>();
        serviceCollection.TryAddScoped<ICasoDeUsoArquivoBaixar, CasoDeUsoArquivoBaixar>();

        serviceCollection.TryAddScoped<IExecutarSincronizacaoInstitucionalDreSyncUseCase, ExecutarSincronizacaoInstitucionalDreSyncUseCase>();
        serviceCollection.TryAddScoped<IExecutarSincronizacaoInstitucionalDreTratarUseCase, ExecutarSincronizacaoInstitucionalDreTratarUseCase>();        
        

        serviceCollection.TryAddScoped<ICasoDeUsoObterFormacaoHomologada, CasoDeUsoObterFormacaoHomologada>();

        serviceCollection.TryAddScoped<ICasoDeUsoObterListaComponentesCurriculares, CasoDeUsoObterListaComponentesCurriculares>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterListaAnoTurma, CasoDeUsoObterListaAnoTurma>();

        serviceCollection.TryAddScoped<IExecutarSincronizacaoComponentesCurricularesEAnosTurmaEOLUseCase, ExecutarSincronizacaoComponentesCurricularesEAnosTurmaEolUseCase>();

        serviceCollection.TryAddScoped<ICasoDeUsoObterModalidade, CasoDeUsoObterModalidade>();
        

        serviceCollection.TryAddScoped<ICasoDeUsoObterListagemFormacaoPaginada, CasoDeUsoObterListagemFormacaoPaginada>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterFormacaoDetalhada, CasoDeUsoObterFormacaoDetalhada>();

        serviceCollection.AddScoped<ICasoDeUsoObterDadosInscricao, CasoDeUsoObterDadosInscricao>();
        
        serviceCollection.TryAddScoped<ICasoDeUsoSalvarInscricao, CasoDeUsoSalvarInscricao>();
        serviceCollection.TryAddScoped<ICasoDeUsoCancelarInscricao, CasoDeUsoCancelarInscricao>();
        serviceCollection.TryAddScoped<ICasoDeUsoTransferirInscricao, CasoDeUsoTransferirInscricao>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterInscricaoPorId, CasoDeUsoObterInscricaoPorId>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterTurmasInscricao, CasoDeUsoObterTurmasInscricao>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterInscricaoPaginada, CasoDeUsoObterInscricaoPaginada>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterDadosPaginadosComFiltros, CasoDeUsoObterDadosPaginadosComFiltros>();
        serviceCollection.TryAddScoped<ICasoDeUsoAlterarVinculoInscricao, CasoDeUsoAlterarVinculoInscricao>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterInformacoesInscricoesEstaoAbertasPorId, CasoDeUsoObterInformacoesInscricoesEstaoAbertasPorId>();

        serviceCollection.TryAddScoped<ICasoDeUsoRealizarInscricaoAutomatica, CasoDeUsoRealizarInscricaoAutomatica>();
        serviceCollection.TryAddScoped<ICasoDeUsoRealizarInscricaoAutomaticaTratarTurmas, CasoDeUsoRealizarInscricaoAutomaticaTratarTurmas>();
        serviceCollection.TryAddScoped<ICasoDeUsoRealizarInscricaoAutomaticaTratarCursista, CasoDeUsoRealizarInscricaoAutomaticaTratarCursista>();
        serviceCollection.TryAddScoped<ICasoDeUsoRealizarInscricaoAutomaticaInscreverCursista, CasoDeUsoRealizarInscricaoAutomaticaInscreverCursista>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterInscricaoTipo, CasoDeUsoObterInscricaoTipo>();

        serviceCollection.TryAddScoped<ICasoDeUsoObterNomeCpfCursistaInscricao, CasoDeUsoObterNomeCpfCpfCursistaInscricao>();

        serviceCollection.TryAddScoped<ICasoDeUsoImportacaoArquivoInscricaoCursista, CasoDeUsoImportacaoInscricaoCursista>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterArquivosInscricaoImportados, CasoDeUsoObterArquivosInscricaoImportados>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterRegistrosDaIncricaoInconsistentes, CasoDeUsoObterRegistrosDaIncricaoInconsistentes>();
        serviceCollection.TryAddScoped<ICasoDeUsoInscricaoManualContinuarProcessamento, CasoDeUsoInscricaoManualContinuarProcessamento>();
        serviceCollection.TryAddScoped<ICasoDeUsoInscricaoManualCancelarProcessamento, CasoDeUsoInscricaoManualCancelarProcessamento>();
        serviceCollection.TryAddScoped<ICasoDeUsoProcessarArquivoDeImportacaoInscricao, CasoDeUsoProcessarArquivoDeImportacaoInscricao>();
        serviceCollection.TryAddScoped<ICasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao, CasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao>();

        serviceCollection.TryAddScoped<ICasoDeUsoSalvarInscricaoManual, CasoDeUsoSalvarInscricaoManual>();

        serviceCollection.TryAddScoped<ICasoDeUsoImportacaoInscricaoCursistaValidar, CasoDeUsoImportacaoInscricaoCursistaValidar>();
        serviceCollection.TryAddScoped<ICasoDeUsoImportacaoInscricaoCursistaValidarItem, CasoDeUsoImportacaoInscricaoCursistaValidarItem>();

        serviceCollection.TryAddScoped<ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista, CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista>();
        serviceCollection.TryAddScoped<ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar, CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterTiposEmail, CasoDeUsoObterTiposEmail>();

        serviceCollection.TryAddScoped<ICasoDeUsoObterUsuariosAdminDf, CasoDeUsoObterUsuariosAdminDf>();

        serviceCollection.TryAddScoped<ICasoDeUsoEnviarEmailDevolverProposta, CasoDeUsoEnviarEmailDevolverProposta>();
        serviceCollection.TryAddScoped<ICasoDeUsoEncerrarInscricaoCursistaInativoSemCargo, CasoDeUsoEncerrarInscricaoCursistaInativoSemCargo>();
        serviceCollection.TryAddScoped<ICasoDeUsoEncerrarInscricaoAutomaticamenteTurma, CasoDeUsoEncerrarInscricaoAutomaticamenteTurma>();
        serviceCollection.TryAddScoped<ICasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes, CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes>();
        serviceCollection.TryAddScoped<ICasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios, CasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterParecerista, CasoDeUsoObterParecerista>();                                        
        serviceCollection.TryAddScoped<ICasoDeUsoObterSituacaoUsuarioRedeParceria, CasoDeUsoObterSituacaoUsuarioRedeParceria>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterUsuarioRedeParceriaPaginada, CasoDeUsoObterUsuarioRedeParceriaPaginada>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterUsuarioRedeParceriaPorId, CasoDeUsoObterUsuarioRedeParceriaPorId>();
        serviceCollection.TryAddScoped<ICasoDeUsoInserirUsuarioRedeParceria, CasoDeUsoInserirUsuarioRedeParceria>();
        serviceCollection.TryAddScoped<ICasoDeUsoAlterarUsuarioRedeParceria, CasoDeUsoAlterarUsuarioRedeParceria>();
        serviceCollection.TryAddScoped<ICasoDeUsoRemoverUsuarioRedeParceria, CasoDeUsoRemoverUsuarioRedeParceria>();

        serviceCollection.TryAddScoped<ICasoDeUsoObterTotalNotificacaoNaoLida, CasoDeUsoObterTotalNotificacaoNaoLida>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterNotificacao, CasoDeUsoObterNotificacao>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterNotificacaoPaginada, CasoDeUsoObterNotificacaoPaginada>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterCategoriaNotificacao, CasoDeUsoObterCategoriaNotificacao>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterTipoNotificacao, CasoDeUsoObterTipoNotificacao>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterSituacaoNotificacao, CasoDeUsoObterSituacaoNotificacao>();

        serviceCollection.TryAddScoped<ICasoDeUsoEnviarEmail, CasoDeUsoEnviarEmail>();
        serviceCollection.TryAddScoped<ICasoDeUsoEnviarNotificacao, CasoDeUsoEnviarNotificacao>();                                                

        serviceCollection.TryAddScoped<ICasoDeUsoConfirmarInscricoes, CasoDeUsoConfirmarInscricoes>();
        serviceCollection.TryAddScoped<ICasoDeUsoEmEsperaInscricoes, CasoDeUsoEmEsperaInscricoes>();
        serviceCollection.TryAddScoped<ICasoDeUsoCancelarInscricoes, CasoDeUsoCancelarInscricoes>();
        serviceCollection.TryAddScoped<ICasoDeUsoSortearInscricoes, CasoDeUsoSortearInscricoes>();

        serviceCollection.TryAddScoped<ICasoDeUsoReativarInscricoes, CasoDeUsoReativarInscricoes>();
        serviceCollection.TryAddScoped<ICasoDeUsoObterUsuariosPorEolUnidade, CasoDeUsoObterUsuariosPorEolUnidade>();

        serviceCollection.AddScoped<IExecutarSincronizacaoCargosEolUseCase, ExecutarSincronizacaoCargosEolUseCase>();
        serviceCollection.AddScoped<ISincronizarCargosEolPorDreUseCase, SincronizarCargosEolPorDreUseCase>();
        serviceCollection.AddScoped<ISincronizarAtribuicoesServidoresEolUseCase, SincronizarAtribuicoesServidoresEolUseCase>();
        serviceCollection.AddScoped<ISincronizarFuncaoAtividadeEolUseCase, SincronizarFuncaoAtividadeEolUseCase>();
        serviceCollection.AddScoped<ISincronizarFuncaoAtividadeEolPorDreUseCase, SincronizarFuncaoAtividadeEolPorDreUseCase>();
        serviceCollection.AddScoped<ICasoDeUsoObterDadosInscricaoParaProposta, CasoDeUsoObterDadosInscricaoParaProposta>();
    }

    protected virtual void RegistrarHttpClients()
    {
        serviceCollection.AdicionarHttpClients(configuration);
    }

    protected virtual void RegistrarServices()
    {
        serviceCollection.AddScoped<IServicoTemplateEmail, ServicoTemplateEmail>();
    }
}