using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.IoC.Features
{
    public static class CoordenadoriaExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AdicionarModuloCoordenadoria() =>
                services
                .AddScoped<IRepositorioCoordenadoria, RepositorioCoordenadoria>()
                ;
        }
    }
}