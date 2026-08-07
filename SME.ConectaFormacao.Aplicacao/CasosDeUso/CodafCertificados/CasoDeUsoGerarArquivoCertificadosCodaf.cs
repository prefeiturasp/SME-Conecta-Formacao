using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados;
using SME.ConectaFormacao.Aplicacao.Interfaces.Utilitarios;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using SME.ConectaFormacao.Infra.Servicos.Relatorio;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados
{
    public class CasoDeUsoGerarArquivoCertificadosCodaf(
        IServicoRelatorio servicoRelatorio,
        IRepositorioCodafCertificado repositorioCodafCertificado,
        IServicoArmazenamento servicoArmazenamento,
        IKeyedServiceProvider serviceProvider,
        IConfiguration configuration,
        IUtilitariosCodaf utilitarios) :
        ICasoDeUsoGerarArquivoCertificadosCodaf
    {
        private readonly IUtilitariosCodaf _utilitarios = utilitarios;
        public async Task<bool> Executar(MensagemRabbit param)
        {
            await _utilitarios.SalvarLogAsync("Início do processamento de certificados Codaf");
            var temCertificadosParaProcessar = true;

            var urlFrontEnd = configuration["UrlFrontEnd"];
            var urlAcessoCertificados = $"{urlFrontEnd?.TrimEnd('/')}/certificados";
            await _utilitarios.SalvarLogAsync($"Url de acesso aos certificados: {urlAcessoCertificados}");

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
                    var tipoEstrategia = _utilitarios.DefinirEstrategia(certificado);
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

                _ = _utilitarios.EnviarEmailsAsync(notificacoesParaEnviar);
            }
            await _utilitarios.SalvarLogAsync("Fim do processamento de certificados Codaf");

            return true;
        }

        private async Task<List<DadosProcessamentoCodafDto>> ProcessarLoteAsync(IEnumerable<DadosProcessamentoCodafDto> codafCertificados)
        {
            var certificadosProcessados = new List<DadosProcessamentoCodafDto>();

            foreach (var certificado in codafCertificados)
            {
                try
                {
                    var htmlComSequencial = _utilitarios.InserirSequencialNoHtml(certificado.HtmlContentSnapshot, certificado.CodigoDeclaracaoOuCertificado);
                    var htmlComSigla = _utilitarios.InserirEmissor(htmlComSequencial, certificado.Emissor);
                    var htmlCertificadoDto = new HtmlCodafDto
                    {
                        HtmlContent = htmlComSigla
                    };
                    var arquivoPdf = await servicoRelatorio.ConveterHtmlCodafParaPdfAsync(htmlCertificadoDto);
                    var certificadoIdGuid = Guid.NewGuid();
                    var nomeDoArquivo = $"{DateTime.Now:yyyy/MM}/{certificado.CodigoDeclaracaoOuCertificado}-{certificadoIdGuid}.pdf";
                    var chaveObjetoArmazenamento = await servicoArmazenamento.UploadCodafAsync(nomeDoArquivo, arquivoPdf);
                    await repositorioCodafCertificado.AtualizarStatusProcessamentoAsync(
                        certificado.Id,
                        StatusProcessamentoCertificadoCodaf.ProcessadoComSucesso,
                        chaveObjetoArmazenamento,
                        null);
                    certificadosProcessados.Add(certificado);
                }
                catch (Exception e)
                {
                    await _utilitarios.SalvarLogAsync($"Erro ao processar certificado Codaf com Id {certificado.Id} e Código {certificado.CodigoDeclaracaoOuCertificado}: {e.Message}", LogNivel.Critico, e);
                    await repositorioCodafCertificado.AtualizarStatusProcessamentoAsync(certificado.Id, StatusProcessamentoCertificadoCodaf.ProcessadoComErro, null, e.Message);
                }
            }
            return certificadosProcessados;
        }       
    }
}
