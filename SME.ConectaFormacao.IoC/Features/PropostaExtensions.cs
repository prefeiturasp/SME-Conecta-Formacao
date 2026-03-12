using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.IoC.Features
{
    public static class PropostaExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AdicionarModuloProposta() =>
            services
            .AddScoped<ICasoDeUsoObterAutocompletarFormacao, CasoDeUsoObterAutocompletarFormacao>()
            .AddScoped<IRepositorioProposta, RepositorioProposta>()
            .AddScoped<IRepositorioPropostaTutor, RepositorioPropostaTutor>()
            .AddScoped<IRepositorioPropostaRegente, RepositorioPropostaRegente>()
            .AddScoped<IRepositorioPropostaMovimentacao, RepositorioPropostaMovimentacao>()
            .AddScoped<IRepositorioPropostaPareceristaConsideracao, RepositorioPropostaPareceristaConsideracao>()
            .AddScoped<ICasoDeUsoObterTipoFormacao, CasoDeUsoObterTipoFormacao>()
            .AddScoped<ICasoDeUsoObterTipoInscricao, CasoDeUsoObterTipoInscricao>()
            .AddScoped<ICasoDeUsoObterFormatos, CasoDeUsoObterFormatos>()
            .AddScoped<ICasoDeUsoInserirProposta, CasoDeUsoInserirProposta>()
            .AddScoped<ICasoDeUsoAlterarProposta, CasoDeUsoAlterarProposta>()
            .AddScoped<ICasoDeUsoObterPropostaPorId, CasoDeUsoObterPropostaPorId>()
            .AddScoped<ICasoDeUsoRemoverProposta, CasoDeUsoRemoverProposta>()
            .AddScoped<ICasoDeUsoObterSituacoesProposta, CasoDeUsoObterSituacoesProposta>()
            .AddScoped<ICasoDeUsoObterPropostaPaginacao, CasoDeUsoObterPropostaPaginacao>()
            .AddScoped<ICasoDeUsoObterInformacoesCadastrante, CasoDeUsoObterInformacoesCadastrante>()
            .AddScoped<ICasoDeUsoObterTurmasProposta, CasoDeUsoObterTurmasProposta>()
            .AddScoped<ICasoDeUsoObterTipoEncontro, CasoDeUsoObterTipoEncontro>()
            .AddScoped<ICasoDeUsoObterPropostaEncontroPaginacao, CasoDeUsoObterPropostaEncontroPaginacao>()
            .AddScoped<ICasoDeUsoObterComunicadoAcaoFormativa, CasoDeUsoObterComunicadoComunicadoAcaoFormativa>()
            .AddScoped<ICasoDeUsoObterNomeRegenteTutor, CasoDeUsoObterNomeRegenteTutor>()
            .AddScoped<ICasoDeUsoSalvarPropostaRegente, CasoDeUsoSalvarPropostaRegente>()
            .AddScoped<ICasoDeUsoObterPropostaRegentePaginacao, CasoDeUsoObterPropostaRegentePaginacao>()
            .AddScoped<ICasoDeUsoObterPropostaRegentePorId, CasoDeUsoObterPropostaRegentePorId>()
            .AddScoped<ICasoDeUsoRemoverPropostaRegente, CasoDeUsoRemoverPropostaRegente>()
            .AddScoped<ICasoDeUsoSalvarPropostaTutor, CasoDeUsoSalvarPropostaTutor>()
            .AddScoped<ICasoDeUsoRemoverPropostaTutor, CasoDeUsoRemoverPropostaTutor>()
            .AddScoped<ICasoDeUsoObterPropostaTutorPaginacao, CasoDeUsoObterPropostaTutorPaginacao>()
            .AddScoped<ICasoDeUsoObterPropostaTutorPorId, CasoDeUsoObterPropostaTutorPorId>()
            .AddScoped<ICasoDeUsoObterPropostasDashboard, CasoDeUsoObterPropostasDashboard>()
            .AddScoped<ICasoDeUsoSalvarPropostaPareceristaConsideracao, CasoDeUsoSalvarPropostaPareceristaConsideracao>()
            .AddScoped<ICasoDeUsoObterPropostaParecer, CasoDeUsoObterPropostaParecer>()
            .AddScoped<ICasoDeUsoObterRelatorioPropostaLaudaPublicacao, CasoDeUsoObterRelatorioPropostaLaudaPublicacao>()
            .AddScoped<ICasoDeUsoObterSugestaoParecerPareceristas, CasoDeUsoObterSugestaoParecerPareceristas>()
            .AddScoped<ICasoDeUsoAprovarProposta, CasoDeUsoAprovarProposta>()
            .AddScoped<ICasoDeUsoRecusarProposta, CasoDeUsoRecusarProposta>()
            .AddScoped<ICasoDeUsoObterRelatorioPropostaLaudaPublicacao, CasoDeUsoObterRelatorioPropostaLaudaPublicacao>()
            .AddScoped<ICasoDeUsoObterRelatorioPropostaLaudaCompleta, CasoDeUsoObterRelatorioPropostaLaudaCompleta>()
            .AddScoped<ICasoDeUsoSalvarPropostaEncontro, CasoDeUsoSalvarPropostaEncontro>()
            .AddScoped<ICasoDeUsoRemoverPropostaEncontro, CasoDeUsoRemoverPropostaEncontro>()
            .AddScoped<ICasoDeUsoEnviarProposta, CasoDeUsoEnviarProposta>()
            .AddScoped<ICasoDeUsoDevolverProposta, CasoDeUsoDevolverProposta>()
            .AddScoped<ICasoDeUsoObterTodosFormatos, CasoDeUsoObterTodosFormatos>()
            .AddScoped<ICasoDeUsoGerarPropostaTurmaVaga, CasoDeUsoGerarPropostaTurmaVaga>()
            .AddScoped<ICasoDeUsoRemoverParecerDaProposta, CasoDeUsoRemoverParecerDaProposta>()
            .AddScoped<ICasoDeUsoEnviarPropostaParecerista, CasoDeUsoEnviarPropostaParecerista>()
            .AddScoped<ICasoDeUsoAprovarPropostaParecerista, CasoDeUsoAprovarPropostaParecerista>()
            .AddScoped<ICasoDeUsoRecusarPropostaParecerista, CasoDeUsoRecusarPropostaParecerista>()
            .AddScoped<ICasoDeUsoObterHorasTotaisProposta, CasoDeUsoObterHorasTotaisProposta>()
            .AddScoped<ICasoDeUsoNotificarPareceristasSobreAtribuicaoPelaDF, CasoDeUsoNotificarPareceristasSobreAtribuicaoPelaDF>()
            .AddScoped<ICasoDeUsoNotificarDFPeloEnvioParecerPeloParecerista, CasoDeUsoNotificarDFPeloEnvioParecerPeloParecerista>()
            .AddScoped<ICasoDeUsoNotificarAreaPromotoraParaAnaliseParecer, CasoDeUsoNotificarAreaPromotoraParaAnaliseParecer>()
            .AddScoped<ICasoDeUsoNotificarPareceristasParaReanalise, CasoDeUsoNotificarPareceristasParaReanalise>()
            .AddScoped<ICasoDeUsoNotificarResponsavelDFSobreReanaliseDoParecerista, CasoDeUsoNotificarResponsavelDFSobreReanaliseDoParecerista>()
            .AddScoped<ICasoDeUsoNotificarAreaPromotoraSobreValidacaoFinalPelaDF, CasoDeUsoNotificarAreaPromotoraSobreValidacaoFinalPelaDF>()
            .AddScoped<ICasoDeUsoObterRoteiroPropostaFormativa, CasoDeUsoObterRoteiroPropostaFormativa>()
            .AddScoped<ICasoDeUsoObterCriterioValidacaoInscricao, CasoDeUsoObterCriterioValidacaoInscricao>()
            ;
        }
    }
}