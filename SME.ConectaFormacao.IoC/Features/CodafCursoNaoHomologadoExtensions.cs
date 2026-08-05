using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.IoC.Features
{
    public static class CodafCursoNaoHomologadoExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddCodafCursoNaoHomologado() =>
                services
                .AddScoped<IRepositorioCodafCursoNaoHomologado, RepositorioCodafCursoNaoHomologado>()
                .AddScoped<ICasoDeUsoListarCodafCursoNaoHomologado, CasoDeUsoListarCodafCursoNaoHomologado>();
        }
    }
}
