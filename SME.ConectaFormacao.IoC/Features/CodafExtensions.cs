using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.IoC.Features
{
    public static class CodafExtensions
    {
        public static IServiceCollection AdicionarModuloCodaf(this IServiceCollection services) =>
            services
            .AddScoped<IRepositorioCodafListaPresenca, RepositorioCodafListaPresenca>()
            .AddScoped<ICasoDeUsoCriarCodafListaPresenca, CasoDeUsoCriarCodafListaPresenca>()
            .AddScoped<ICasoDeUsoAtualizarCodafListaPresenca, CasoDeUsoAtualizarCodafListaPresenca>()
            ;
    }
}