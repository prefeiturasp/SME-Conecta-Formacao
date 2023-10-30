using System.Net;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol.Constante;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Infra.Servicos.Eol
{
    public class ServicoEol : IServicoEol
    {
        private readonly HttpClient _httpClient;
        public ServicoEol(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        
        public async Task<string> ObterNomeProfissionalPorRegistroFuncional(string registroFuncional)
        {
            var resposta = await _httpClient.GetAsync(string.Format(ServicoEolConstantes.OBTER_NOME_PROFISSIONAL, registroFuncional));
            if (resposta.IsSuccessStatusCode && resposta.StatusCode != HttpStatusCode.NoContent)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                return json.JsonParaObjeto<string>();
            }
            else
                throw new NegocioException(MensagemNegocio.PROFISSIONAL_NAO_LOCALIZADO, resposta.StatusCode);
            
        }
    }
}