using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol.Constante;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;
using System.Net;

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
            
            if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
                throw new NegocioException(MensagemNegocio.PROFISSIONAL_NAO_LOCALIZADO, resposta.StatusCode);
            
            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<string>().ToUpper();

        }

        public async Task<IEnumerable<DreNomeAbreviacaoDTO>> ObterCodigosDres()
        {
            var resposta = await _httpClient.GetAsync(ServicoEolConstantes.OBTER_NOME_ABREVIACAO_DRE);
            
            if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
                throw new NegocioException(MensagemNegocio.CODIGOS_DRE_NAO_LOCALIZADO, resposta.StatusCode);
            
            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<DreNomeAbreviacaoDTO[]>().ToList();

        }

        public async Task<IEnumerable<ComponenteCurricularAnoTurmaEOLDTO>> ObterComponentesCurricularesEAnoPorAnoLetivo(int anoLetivo)
        {
            var resposta = await _httpClient.GetAsync(string.Format(ServicoEolConstantes.OBTER_COMPONENTE_CURRICULAR_E_ANO_TURMA_POR_ANO_LETIVO, anoLetivo));
            
            if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
                throw new NegocioException(MensagemNegocio.NENHUM_COMPONENTE_CURRICULAR_DO_EOL_FOI_LOCALIZADOS, resposta.StatusCode);
            
            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<ComponenteCurricularAnoTurmaEOLDTO[]>().ToList();

        }
    }
}