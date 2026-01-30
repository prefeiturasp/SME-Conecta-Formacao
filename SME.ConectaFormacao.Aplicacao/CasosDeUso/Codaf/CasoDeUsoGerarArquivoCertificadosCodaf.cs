using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using SME.ConectaFormacao.Infra.Servicos.Relatorio;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
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

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var temCertificadosParaProcessar = true;

            var urlFrontEnd = configuration["UrlFrontEnd"];
            var urlAcessoCertificados = $"{urlFrontEnd?.TrimEnd('/')}/meus-certificados";

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
                    var htmlCertificadoDto = new HtmlCertificadoCodafDto
                    {
                        HtmlContent = htmlComSequencial
                    };
                    var arquivoPdf = await servicoRelatorio.ConveterHtmlCertificadoCodafParaPdfAsync(htmlCertificadoDto);
                    var certificadoIdGuid = Guid.NewGuid();
                    var nomeDoArquivo = $"{DateTime.Now:yyyy/MM}/{certificado.CodigoCertificado:D4}-{certificadoIdGuid}.pdf";
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
                    await repositorioCodafCertificado.AtualizarStatusProcessamentoAsync(certificado.Id, StatusProcessamentoCertificadoCodaf.ProcessadoComErro, null, e.Message);
                }
            }
            return certificadosProcessados;
        }

        private static string InserirSequencialNoHtml(string htmlContent, long sequencial)
        {
            var sequencialFormatado = sequencial.ToString("D4");
            var marcador = "{{NUM_SEQ}}";
            if (htmlContent.Contains(marcador))
                htmlContent = htmlContent.Replace(marcador, sequencialFormatado);
            return htmlContent;
        }

        private static TipoEstrategiaCertificadoCodaf DefinirEstrategia(DadosProcessamentoCertificadoCodafDto certificado)
        {
            if (certificado.TipoParticipacao == TipoParticipacaoCodaf.Regente)
                return TipoEstrategiaCertificadoCodaf.Regente;

            return certificado.TemRf ? TipoEstrategiaCertificadoCodaf.CursistaComRf : TipoEstrategiaCertificadoCodaf.CursistaSemRf;
        }

        private async Task EnviarEmailsAsync(List<EnviarEmailDto> notificacoesParaEnviar)
        {
            foreach (var emailDto in notificacoesParaEnviar)
            {
                _ = mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, emailDto));
            }
        }
    }
}
