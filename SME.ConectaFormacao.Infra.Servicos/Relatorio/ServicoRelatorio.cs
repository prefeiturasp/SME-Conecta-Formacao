using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Constantes;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Options;
using System.Net;

namespace SME.ConectaFormacao.Infra.Servicos.Relatorio
{
    public class ServicoRelatorio : IServicoRelatorio
    {
        private readonly HttpClient _httpClient;
        private readonly RelatorioOptions _relatorioOptions;

        public ServicoRelatorio(HttpClient httpClient, RelatorioOptions relatorioOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _relatorioOptions = relatorioOptions ?? throw new ArgumentNullException(nameof(relatorioOptions));
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
