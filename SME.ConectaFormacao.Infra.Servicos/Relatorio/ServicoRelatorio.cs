using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Constantes;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;

namespace SME.ConectaFormacao.Infra.Servicos.Relatorio
{
    [ExcludeFromCodeCoverage]
    public class ServicoRelatorio : IServicoRelatorio
    {
        private readonly HttpClient _httpClient;
        private readonly RelatorioOptions _relatorioOptions;

        public ServicoRelatorio(HttpClient httpClient, RelatorioOptions relatorioOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _relatorioOptions = relatorioOptions ?? throw new ArgumentNullException(nameof(relatorioOptions));
        }

        public async Task<byte[]> ConveterHtmlCertificadoCodafParaPdfAsync(HtmlCertificadoCodafDto htmlCertificadoCodafDto)
        {
            var resposta = await _httpClient.PostAsJsonAsync(EndpointRelatoriosConstants.RELATORIO_CERTIFICADO_CODAF, htmlCertificadoCodafDto);
            resposta.EnsureSuccessStatusCode();
            var arquivoBytes = await resposta.Content.ReadAsByteArrayAsync();
            return arquivoBytes;
        }

        public async Task<Resultado<Stream>> ConveterHtmlCertificadoCodafParaPdfAsync(HtmlCertificadoCodafDto htmlCertificadoCodafDto, CancellationToken cancellationToken)
        {
            try
            {
                var resposta = await _httpClient.PostAsJsonAsync(
                    EndpointRelatoriosConstants.RELATORIO_CERTIFICADO_CODAF,
                    htmlCertificadoCodafDto,
                    cancellationToken);

                if (!resposta.IsSuccessStatusCode)
                {
                    var erroDetalhe = await resposta.Content.ReadAsStringAsync(cancellationToken);
                    return new Erro(TipoFalha.ErroInterno, $"A API de relatórios retornou {resposta.StatusCode}. Detalhe: {erroDetalhe}");
                }

                if (resposta.StatusCode == HttpStatusCode.NoContent)
                {
                    return Erro.NaoEncontrado(MensagemNegocio.ARQUIVO_NAO_ENCONTRADO);
                }

                using var stream = await resposta.Content.ReadAsStreamAsync(cancellationToken);
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream, cancellationToken);
                memoryStream.Position = 0;

                return memoryStream;
            }
            catch (Exception ex)
            {
                return new Erro(TipoFalha.ErroInterno, $"Ocorreu um erro ao converter o certificado Codaf para PDF: {ex.Message}");
            }
        }

        public async Task<byte[]> GerarRelatorioCodafAsync(long codafId)
        {
            var resposta = await _httpClient.PostAsync(EndpointRelatoriosConstants.RELATORIO_CODAF.Parametros(codafId), null);
            if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
                throw new NegocioException(MensagemNegocio.ARQUIVO_NAO_ENCONTRADO, resposta.StatusCode);
            var arquivoBytes = await resposta.Content.ReadAsByteArrayAsync();
            return arquivoBytes;
        }

        public Task<string> ObterRelatorioPropostaLaudaCompleta(long propostaId)
        {
            return ObterRelatorio(EndpointRelatoriosConstants.RELATORIO_LAUDA_COMPLETA.Parametros(propostaId), "pdf");
        }

        public Task<string> ObterRelatorioPropostaLaudaDePublicacao(long propostaId)
        {
            return ObterRelatorio(EndpointRelatoriosConstants.RELATORIO_LAUDA_PUBLICACAO.Parametros(propostaId), "doc");
        }

        private async Task<string> ObterRelatorio(string endPoint, string extensao)
        {
            var resposta = await _httpClient.GetAsync(endPoint);

            if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
                throw new NegocioException(MensagemNegocio.ARQUIVO_NAO_ENCONTRADO, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();

            var nomeArquivo = json.JsonParaObjeto<string>();

            return ObterUrl(nomeArquivo, extensao);
        }

        private string ObterUrl(string nomeArquivo, string extensao)
        {
            return $"{_relatorioOptions.UrlApiServidorRelatorios.Trim()}v1/downloads/conecta/{extensao}/{nomeArquivo}";
        }
    }
}
