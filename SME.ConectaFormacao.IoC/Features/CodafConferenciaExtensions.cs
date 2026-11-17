using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Servicos;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.IoC.Features
{
    [ExcludeFromCodeCoverage]
    public static class CodafConferenciaExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddCodafConferencia() =>
                services
                .AddScoped<IRepositorioCodafConferencia, RepositorioCodafConferencia>()
                .AddScoped<IRepositorioCodafConferenciaAnexo, RepositorioCodafConferenciaAnexo>()
                .AddScoped<IRepositorioCodafConferenciaInscricao, RepositorioCodafConferenciaInscricao>()
                .AddScoped<ICasoDeUsoListarCodafConferencia, CasoDeUsoListarCodafConferencia>()
                .AddScoped<ICasoDeUsoListarInscritosTurmaCodafConferencia, CasoDeUsoListarInscritosTurmaCodafConferencia>()
                .AddScoped<ICasoDeUsoCriarCodafConferencia, CasoDeUsoCriarCodafConferencia>()
                .AddScoped<ICasoDeUsoAtualizarCodafConferencia, CasoDeUsoAtualizarCodafConferencia>()
                .AddScoped<ICasoDeUsoExcluirCodafConferencia, CasoDeUsoExcluirCodafConferencia>()
                .AddScoped<ICasoDeUsoObterCodafConferenciaPorId, CasoDeUsoObterCodafConferenciaPorId>()
                .AddScoped<ICasoDeUsoObterDetalhesPropostaConferenciaComTurmasPorId, CasoDeUsoObterDetalhesPropostaConferenciaComTurmasPorId>()
                .AddScoped<ICasoDeUsoTurmaPossuiCodafConferencia, CasoDeUsoTurmaPossuiCodafConferencia>()
                .AddScoped<ICasoDeUsoUploadAnexoTemporarioCodafConferencia, CasoDeUsoUploadAnexoTemporarioCodafConferencia>()
                .AddScoped<CodafConferenciaDependencias>()
                .AddScoped<IGerenciadorAnexosCodafConferenciaService, GerenciadorAnexosCodafConferenciaService>()
                .AddScoped<ICodafConferenciaInscritosService, CodafConferenciaInscritosService>();
        }
    }
}
