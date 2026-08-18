using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Relatorios;
using SME.ConectaFormacao.Aplicacao.Interfaces.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Relatorios.Codaf.Gerador;
using SME.ConectaFormacao.Infra.Dados.Relatorios.Codaf.Gerador.Intefaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.IoC.Features
{
    public static class RelatorioExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AdicionarModuloRelatorio() =>
                services
                    .AddScoped<IRepositorioRelatorios, RepositorioRelatorios>()
                    .AddScoped<IGeradorRelatorioInscritosExcelService, GeradorRelatorioInscritosExcelService>()
                    .AddScoped<IGeradorRelatorioCodafSuplementarExcelService, GeradorRelatorioCodafSuplementarExcelService>()
                    .AddScoped<ICasoDeUsoGerarRelatorioInscritosPorFormacao, CasoDeUsoGerarRelatorioInscritosPorFormacao>()
                    .AddScoped<ICasoDeUsoSolicitarGeracaoRelatorioInscritosPorFormacao, CasoDeUsoSolicitarGeracaoRelatorioInscritosPorFormacao>()
                    .AddSingleton<IBlocoTituloGerador, BlocoTituloGerador>()
                    .AddSingleton<IBlocoCabecalhoGerador, BlocoCabecalhoGerador>()
                    .AddSingleton<IBlocoRegentesGerador, BlocoRegentesGerador>()
                    .AddSingleton<IBlocoAlunosGerador, BlocoAlunosGerador>()
                    .AddSingleton<IBlocoAssinaturaGerador, BlocoAssinaturaGerador>()
                ;
        }
    }
}
