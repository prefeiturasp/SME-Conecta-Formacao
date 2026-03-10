using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Relatorios;
using SME.ConectaFormacao.Aplicacao.Interfaces.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Relatorios;

namespace SME.ConectaFormacao.IoC.Features
{
    public static class RelatorioExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AdicionarModuloRelatorio() =>
                services
                    .AddScoped<IGeradorRelatorioInscritosExcelService, GeradorRelatorioInscritosExcelService>()
                    .AddScoped<ICasoDeUsoGerarRelatorioInscritosUseCase, CasoDeUsoGerarRelatorioInscritosUseCase>()
                 ;
        }
    }
}
