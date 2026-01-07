using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Servicos;

namespace SME.ConectaFormacao.IoC.Features
{
    public static class CodafExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AdicionarModuloCodaf() =>
            services
                .AddScoped<IRepositorioCodafListaPresenca, RepositorioCodafListaPresenca>()
                .AddScoped<IValidadorCodafListaPresencaService, ValidadorCodafListaPresencaService>()
                .AddScoped<ICasoDeUsoCriarCodafListaPresenca, CasoDeUsoCriarCodafListaPresenca>()
                .AddScoped<ICasoDeUsoAtualizarCodafListaPresenca, CasoDeUsoAtualizarCodafListaPresenca>()
                .AddScoped<ICasoDeUsoListarCodafListaPresenca, CasoDeUsoListarCodafListaPresenca>()
                .AddScoped<ICasoDeUsoObterCodafListaPresencaPorId, CasoDeUsoObterCodafListaPresencaPorId>()
                .AddScoped<IRepositorioCodafInscritosListaPresenca, RepositorioCodafInscritosListaPresenca>()
                .AddScoped<ICasoDeUsoListarInscritosTurmaCodafListaPresenca, CasoDeUsoListarInscritosTurmaCodafListaPresenca>()
                .AddScoped<ICasoDeUsoTurmaPossuiCodafListaPresenca, CasoDeUsoTurmaPossuiCodafListaPresenca>()
                .AddScoped<IRepositorioCodafRetificacaoListaPresenca, RepositorioCodafRetificacaoListaPresenca>()
                .AddScoped<ICasoDeUsoRemoverCodafRetificacaoListaPresenca, CasoDeUsoRemoverCodafRetificacaoListaPresenca>()
            ;
        }
    }
}