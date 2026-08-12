using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Servicos;

namespace SME.ConectaFormacao.IoC.Features
{
    public static class CodafCursoNaoHomologadoExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddCodafCursoNaoHomologado() =>
                services
                .AddScoped<IRepositorioCodafCursoNaoHomologado, RepositorioCodafCursoNaoHomologado>()
                .AddScoped<IRepositorioCodafCursoNaoHomologadoAnexo, RepositorioCodafCursoNaoHomologadoAnexo>()
                .AddScoped<IRepositorioCodafCursoNaoHomologadoInscricao, RepositorioCodafCursoNaoHomologadoInscricao>()
                .AddScoped<ICasoDeUsoListarCodafCursoNaoHomologado, CasoDeUsoListarCodafCursoNaoHomologado>()
                .AddScoped<ICasoDeUsoListarInscritosTurmaCodafCursoNaoHomologado, CasoDeUsoListarInscritosTurmaCodafCursoNaoHomologado>()
                .AddScoped<ICasoDeUsoCriarCodafCursoNaoHomologado, CasoDeUsoCriarCodafCursoNaoHomologado>()
                .AddScoped<CodafCursoNaoHomologadoDependencias>()
                .AddScoped<IGerenciadorAnexosCodafCursoNaoHomologadoService, GerenciadorAnexosCodafCursoNaoHomologadoService>()
                .AddScoped<ICodafCursoNaoHomologadoInscritosService, CodafCursoNaoHomologadoInscritosService>()
                .AddScoped<ICasoDeUsoObterCodafCursoNaoHomologadoPorId, CasoDeUsoObterCodafCursoNaoHomologadoPorId>()
                ;
        }
    }
}
