using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafDeclaracoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Utilitarios;
using SME.ConectaFormacao.Aplicacao.Utilitarios;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Estrategias.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias.CodafDeclaracoes;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Servicos;

namespace SME.ConectaFormacao.IoC.Features
{
    public static class CodafExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AdicionarModuloCodaf() =>
            services
                .AddScoped<IRepositorioCodafMovimentacaoListaPresenca, RepositorioCodafMovimentacaoListaPresenca>()
                .AddScoped<IRepositorioCodafListaPresenca, RepositorioCodafListaPresenca>()
                .AddScoped<IValidadorCodafListaPresencaService, ValidadorCodafListaPresencaService>()
                .AddScoped<ICasoDeUsoCriarCodafListaPresenca, CasoDeUsoCriarCodafListaPresenca>()
                .AddScoped<ICasoDeUsoAtualizarCodafListaPresenca, CasoDeUsoAtualizarCodafListaPresenca>()
                .AddScoped<ICasoDeUsoListarCodafListaPresenca, CasoDeUsoListarCodafListaPresenca>()
                .AddScoped<ICasoDeUsoObterCodafListaPresencaPorId, CasoDeUsoObterCodafListaPresencaPorId>()
                .AddScoped<IRepositorioCodafInscritosListaPresenca, RepositorioCodafInscritosListaPresenca>()
                .AddScoped<ICasoDeUsoListarInscritosTurmaCodafListaPresenca, CasoDeUsoListarInscritosTurmaCodafListaPresenca>()
                .AddScoped<ICasoDeUsoTurmaPossuiCodafListaPresenca, CasoDeUsoTurmaPossuiCodafListaPresenca>()
                .AddScoped<IRepositorioCodafRetificacaoListaPresenca, RepositorioCodafRetificacaoListaPresenca>()
                .AddScoped<ICasoDeUsoRemoverCodafRetificacaoListaPresenca, CasoDeUsoRemoverCodafRetificacaoListaPresenca>()
                .AddScoped<ICasoDeUsoObterModeloTermoResponsabilidadeCodaf, CasoDeUsoObterModeloTermoResponsabilidadeCodaf>()
                .AddScoped<ICasoDeUsoUploadAnexoTemporarioCodafListaPresenca, CasoDeUsoUploadAnexoTemporarioCodafListaPresenca>()
                .AddScoped<IRepositorioCodafAnexo, RepositorioCodafAnexo>()
                .AddScoped<IGerenciadorAnexosCodafService, GerenciadorAnexosCodafService>()
                .AddScoped<IGerenciadorMovimentacaoCodafService, GerenciadorMovimentacaoCodafService>()
                .AddScoped<ICasoDeUsoEnviarParaDfCodafListaPresenca, CasoDeUsoEnviarParaDfCodafListaPresenca>()
                .AddScoped<IRepositorioCodafComentarioListaPresenca, RepositorioCodafComentarioListaPresenca>()
                .AddScoped<ICasoDeUsoDevolverParaCorrecaoCodafListaPresenca, CasoDeUsoDevolverParaCorrecaoCodafListaPresenca>()
                .AddScoped<ICasoDeUsoExcluirCodafListaPresenca, CasoDeUsoExcluirCodafListaPresenca>()
                .AddScoped<IRepositorioCodafLogRemessaConclusao, RepositorioCodafLogRemessaConclusao>()
                .AddScoped<ICasoDeUsoGerarArquivoRemessaConclusaoCodaf, CasoDeUsoGerarArquivoRemessaConclusaoCodaf>()
                .AddScoped<ICodafInscritosListaPresencaService, CodafInscritosListaPresencaService>()
                .AddScoped<CodafListaPresencaDependencias>()
                .AddScoped<ICasoDeUsoSalvarInscritosCodaf, CasoDeUsoSalvarInscritosCodaf>()
                .AddScoped<ICasoDeUsoObterPropostaTurmaComCodaf, CasoDeUsoObterPropostaTurmaComCodaf>()
                .AddScoped<ICasoDeUsoFinalizarCodafListaPresenca, CasoDeUsoFinalizarCodafListaPresenca>()
                .AddScoped<IUtilitariosCodaf, UtilitariosCodaf>()
                .AdicionarModuloCodafCertificado()
                .AdicionarModuloCodafDeclaracao();

            public IServiceCollection AdicionarModuloCodafCertificado() =>
                services
                    .AddScoped<IRepositorioCodafCertificado, RepositorioCodafCertificado>()
                    .AddKeyedScoped<ICertificadoCodafGeradorConteudo, CertificadoCursistaComRfStrategy>(TipoEstrategiaCodaf.CursistaComRf)
                    .AddKeyedScoped<ICertificadoCodafGeradorConteudo, CertificadoCursistaSemRfStrategy>(TipoEstrategiaCodaf.CursistaSemRf)
                    .AddKeyedScoped<ICertificadoCodafGeradorConteudo, CertificadoRegenteComRfStrategy>(TipoEstrategiaCodaf.RegenteComRf)
                    .AddKeyedScoped<ICertificadoCodafGeradorConteudo, CertificadoRegenteSemRfStrategy>(TipoEstrategiaCodaf.RegenteSemRf)
                    .AddScoped<ICasoDeUsoEmitirCertificadoCodaf, CasoDeUsoEmitirCertificadoCodaf>()
                    .AddScoped<ICasoDeUsoGerarArquivoCertificadosCodaf, CasoDeUsoGerarArquivoCertificadosCodaf>()
                    .AddScoped<ICasoDeUsoRecuperarCertificadosTravadosCodafResiliencia, CasoDeUsoRecuperarCertificadosTravadosCodafResiliencia>()
                    .AddScoped<ICasoDeUsoListarMeusCertificadosCodaf, CasoDeUsoListarMeusCertificadosCodaf>()
                    .AddScoped<ICasoDeUsoObterCertificadoCodafParaDownload, CasoDeUsoObterCertificadoCodafParaDownload>()
                    .AddScoped<ICasoDeUsoGerarRelatorioCodaf, CasoDeUsoGerarRelatorioCodaf>()
                    .AddScoped<ICasoDeUsoListarTodosCertificadosCodaf, CasoDeUsoListarTodosCertificadosCodaf>()
                    .AddScoped<ICasoDeUsoDownloadLoteCertificados, CasoDeUsoDownloadLoteCertificados>();

            public IServiceCollection AdicionarModuloCodafDeclaracao() =>
                services
                    .AddScoped<IRepositorioCodafDeclaracao, RepositorioCodafDeclaracao>()
                    .AddKeyedScoped<IDeclaracaoCodafGeradorConteudo, DeclaracaoCursistaComRfStrategy>(TipoEstrategiaCodaf.CursistaComRf)
                    .AddKeyedScoped<IDeclaracaoCodafGeradorConteudo, DeclaracaoCursistaSemRfStrategy>(TipoEstrategiaCodaf.CursistaSemRf)
                    .AddKeyedScoped<IDeclaracaoCodafGeradorConteudo, DeclaracaoRegenteComRfStrategy>(TipoEstrategiaCodaf.RegenteComRf)
                    .AddKeyedScoped<IDeclaracaoCodafGeradorConteudo, DeclaracaoRegenteSemRfStrategy>(TipoEstrategiaCodaf.RegenteSemRf)
                    .AddScoped<ICasoDeUsoEmitirDeclaracaoCodaf, CasoDeUsoEmitirDeclaracaoCodaf>()
                    .AddScoped<ICasoDeUsoGerarArquivoDeclaracoesCodaf, CasoDeUsoGerarArquivoDeclaracoesCodaf>()
                    .AddScoped<ICasoDeUsoListarMinhasDeclaracoesCodaf, CasoDeUsoListarMinhasDeclaracoesCodaf>()
                    .AddScoped<ICasoDeUsoObterDeclaracaoCodafParaDownload, CasoDeUsoObterDeclaracaoCodafParaDownload>()
                    .AddScoped<ICasoDeUsoListarTodasDeclaracoesCodaf, CasoDeUsoListarTodasDeclaracoesCodaf>()
                    .AddScoped<ICasoDeUsoDownloadLoteDeclaracoes, CasoDeUsoDownloadLoteDeclaracoes>();
        }
    }
}
