using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Servicos;

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
                .AddScoped<ICasoDeUsoObterCodafSuplementarPorCodafId, CasoDeUsoObterCodafSuplementarPorCodafId>()
                .AddScoped<ICasoDeUsoCriarCodafSuplementar, CasoDeUsoCriarCodafSuplementar>()
                .AddScoped<ICasoDeUsoListarCodafSuplementar, CasoDeUsoListarCodafSuplementar>()
                .AddScoped<ICasoDeUsoObterCodafSuplementarPorId, CasoDeUsoObterCodafSuplementarPorId>()
                .AddScoped<ICodafSuplementarInscritosService, CodafSuplementarInscritosService>()
                .AddScoped<ICasoDeUsoAtualizarCodafSuplementar, CasoDeUsoAtualizarCodafSuplementar>()
                .AddScoped<ICasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar, CasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar>()
                .AddScoped<ICasoDeUsoUploadAnexoTemporarioCodafSuplementar, CasoDeUsoUploadAnexoTemporarioCodafSuplementar>()
                .AddScoped<ICasoDeUsoExcluirCodafSuplementar, CasoDeUsoExcluirCodafSuplementar>()
                .AddScoped<ICasoDeUsoRemoverCodafSuplementarRetificacao, CasoDeUsoRemoverCodafSuplementarRetificacao>()
                .AddScoped<IGerenciadorAnexosCodafSuplementarService, GerenciadorAnexosCodafSuplementarService>()
                .AddScoped<CodafSuplementarDependencias>()
                ;
        }
    }
}
