using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.IoC.Features
{
    public static class CodafSuplementarExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddCodafSuplementar() =>
                services
                .AddScoped<IRepositorioCodafSuplementarAnexo, RepositorioCodafSuplementarAnexo>()
                .AddScoped<IRepositorioCodafSuplementarLogRemessaConclusao, RepositorioCodafSuplementarLogRemessaConclusao>()
                .AddScoped<IRepositorioCodafSuplementarRetificacao, RepositorioCodafSuplementarRetificacao>()
                .AddScoped<IRepositorioCodafSuplementar, RepositorioCodafSuplementar>()
                .AddScoped<IRepositorioCodafSuplementarInscricao, RepositorioCodafSuplementarInscricao>()
                ;
        }
    }
}
