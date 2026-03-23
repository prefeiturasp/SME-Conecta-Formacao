using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Ues;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ues;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.IoC.Features
{
    public static class UeExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AdicionarModuloUe() =>
            services
            .AddScoped<IRepositorioUe, RepositorioUe>()
            .AddScoped<ICasoDeUsoObterAutocompletarNomeUe, CasoDeUsoObterAutocompletarNomeUe>()
            ;
        }
    }
}