using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using SME.ConectaFormacao.Infra.Servicos.Relatorio;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;
using System.Drawing.Printing;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados
{
    public class CasoDeUsoGerarArquivoCertificadosCodaf(
        IServicoRelatorio servicoRelatorio,
        IRepositorioCodafCertificado repositorioCodafCertificado,
        IServicoArmazenamento servicoArmazenamento,
        IMediator mediator,
        IKeyedServiceProvider serviceProvider,
        IConfiguration configuration) :
        ICasoDeUsoGerarArquivoCertificadosCodaf
    {
        private readonly Guid _identificadorRastreamento = Guid.NewGuid();

        public async Task<bool> Executar(MensagemRabbit param)
        {
            await SalvarLogAsync("Início do processamento de certificados Codaf");
            var temCertificadosParaProcessar = true;

            var urlFrontEnd = configuration["UrlFrontEnd"];
            var urlAcessoCertificados = $"{urlFrontEnd?.TrimEnd('/')}/certificados";
            await SalvarLogAsync($"Url de acesso aos certificados: {urlAcessoCertificados}");

            while (temCertificadosParaProcessar)
            {
                var notificacoesParaEnviar = new List<EnviarEmailDto>();
                var loteCertificados = await repositorioCodafCertificado.ObterCertificadosParaProcessamentoAsync();
                if (!loteCertificados.Any())
                {
                    temCertificadosParaProcessar = false;
                    continue;
                }
                var certificadosProcessados = await ProcessarLoteAsync(loteCertificados);

                foreach (var certificado in certificadosProcessados)
                {
                    var tipoEstrategia = DefinirEstrategia(certificado);
                    var geradorCertificado = serviceProvider.GetRequiredKeyedService<ICertificadoCodafGeradorConteudo>(tipoEstrategia);

                    var (tituloEmail, textoEmail) = geradorCertificado.GerarConteudoEmail(certificado, urlAcessoCertificados);

                    if (!string.IsNullOrEmpty(certificado.EmailUsuario))
                    {
                        notificacoesParaEnviar.Add(new()
                        {
                            EmailDestinatario = certificado.EmailUsuario!,
                            NomeDestinatario = certificado.NomeCompleto,
                            Texto = textoEmail,
                            Titulo = tituloEmail
                        });
                    }
                }

                _ = EnviarEmailsAsync(notificacoesParaEnviar);
            }
            await SalvarLogAsync("Fim do processamento de certificados Codaf");

            return true;
        }

        private async Task<List<DadosProcessamentoCertificadoCodafDto>> ProcessarLoteAsync(IEnumerable<DadosProcessamentoCertificadoCodafDto> codafCertificados)
        {
            var certificadosProcessados = new List<DadosProcessamentoCertificadoCodafDto>();

            foreach (var certificado in codafCertificados)
            {
                try
                {
                    var htmlComSequencial = InserirSequencialNoHtml(certificado.HtmlContentSnapshot, certificado.CodigoCertificado);
                    var htmlComSigla = InserirEmissor(htmlComSequencial, certificado.Emissor);
                    var htmlCertificadoDto = new HtmlCertificadoCodafDto
                    {
                        HtmlContent = htmlComSigla
                    };
                    var arquivoPdf = await servicoRelatorio.ConveterHtmlCertificadoCodafParaPdfAsync(htmlCertificadoDto);
                    var certificadoIdGuid = Guid.NewGuid();
                    var nomeDoArquivo = $"{DateTime.Now:yyyy/MM}/{certificado.CodigoCertificado}-{certificadoIdGuid}.pdf";
                    var chaveObjetoArmazenamento = await servicoArmazenamento.UploadCertificadoCodafAsync(nomeDoArquivo, arquivoPdf);
                    await repositorioCodafCertificado.AtualizarStatusProcessamentoAsync(
                        certificado.Id,
                        StatusProcessamentoCertificadoCodaf.ProcessadoComSucesso,
                        chaveObjetoArmazenamento,
                        null);
                    certificadosProcessados.Add(certificado);
                }
                catch (Exception e)
                {
                    await SalvarLogAsync($"Erro ao processar certificado Codaf com Id {certificado.Id} e Código {certificado.CodigoCertificado}: {e.Message}", LogNivel.Critico, e);
                    await repositorioCodafCertificado.AtualizarStatusProcessamentoAsync(certificado.Id, StatusProcessamentoCertificadoCodaf.ProcessadoComErro, null, e.Message);
                }
            }
            return certificadosProcessados;
        }

        private static string InserirSequencialNoHtml(string htmlContent, long sequencial)
        {
            var marcador = "{{NUM_SEQ}}";
            if (htmlContent.Contains(marcador))
                htmlContent = htmlContent.Replace(marcador, sequencial.ToString());
            return htmlContent;
        }

        private static string InserirEmissor(string htmlContent, string sigla)
        {
            var marcador = "{{EMISSOR}}";
            if (htmlContent.Contains(marcador))
                htmlContent = htmlContent.Replace(marcador, sigla);
            return htmlContent;
        }

        private static TipoEstrategiaCertificadoCodaf DefinirEstrategia(DadosProcessamentoCertificadoCodafDto certificado)
        {
            if (certificado.TipoParticipacao == TipoParticipacaoCodaf.Regente)
                return TipoEstrategiaCertificadoCodaf.RegenteComRf;

            return certificado.TemRf ? TipoEstrategiaCertificadoCodaf.CursistaComRf : TipoEstrategiaCertificadoCodaf.CursistaSemRf;
        }

        private async Task EnviarEmailsAsync(List<EnviarEmailDto> notificacoesParaEnviar)
        {
            foreach (var emailDto in notificacoesParaEnviar)
            {
                _ = mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, emailDto));
            }
        }

        private async Task SalvarLogAsync(string mensagem, LogNivel nivelLog = LogNivel.Informacao, Exception? ex = null)
        {
            try
            {
                var complemento = MontarComplementoExcecao(ex);
                await mediator.Send(new SalvarLogCommand(
                    entidade: typeof(CasoDeUsoGerarArquivoCertificadosCodaf).FullName!,
                    nivelLog: nivelLog, mensagem: $"[{_identificadorRastreamento}] {mensagem}", complemento: complemento));
            }
            catch
            {
                // Ignorar erros ao salvar log para não interromper o processamento
            }
        }

        private static string MontarComplementoExcecao(Exception ex, int nivel = 0)
        {
            var complemento = string.Empty;
            if (ex is not null)
                complemento = $"{nivel + 1}: Exception: {ex.Message} | StackTrace: {ex.StackTrace}";

            if (ex is not null && ex.InnerException is not null)
                complemento += Environment.NewLine + MontarComplementoExcecao(ex.InnerException, nivel + 1);

            return complemento;
        }
    }
}
