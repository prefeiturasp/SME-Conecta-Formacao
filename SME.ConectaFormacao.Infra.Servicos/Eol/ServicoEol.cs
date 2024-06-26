using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol.Constante;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;
using System.Net;
using System.Text;

namespace SME.ConectaFormacao.Infra.Servicos.Eol
{
    public class ServicoEol : IServicoEol
    {
        private readonly HttpClient _httpClient;

        public ServicoEol(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }


        public async Task<CursistaResumidoServicoEol> ObterNomeCpfProfissionalPorRegistroFuncional(string registroFuncional)
        {
            var resposta = await _httpClient.GetAsync(EndpointsEolConstantes.OBTER_NOME_PROFISSIONAL.Parametros(registroFuncional));

            if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
                throw new NegocioException(MensagemNegocio.PROFISSIONAL_NAO_LOCALIZADO, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<CursistaResumidoServicoEol>();
        }

        public async Task<IEnumerable<DreServicoEol>> ObterCodigosDres()
        {
            var resposta = await _httpClient.GetAsync(EndpointsEolConstantes.OBTER_NOME_ABREVIACAO_DRE);

            if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
                throw new NegocioException(MensagemNegocio.CODIGOS_DRE_NAO_LOCALIZADO, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<DreServicoEol[]>();
        }
        public async Task<UnidadeEol> ObterUnidadePorCodigoEol(string codigoEol)
        {
            var resposta = await _httpClient.GetAsync(EndpointsEolConstantes.OBTER_UNIDADE_POR_CODIGO.Parametros(codigoEol));

            if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
                throw new NegocioException(MensagemNegocio.UNIDADE_NAO_LOCALIZADA_POR_CODIGO);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<UnidadeEol>();
        }

        public async Task<IEnumerable<FuncionarioExternoServicoEol>?> ObterDadosFuncionarioExternoPorCpf(string cpf)
        {
            var resposta = await _httpClient.GetAsync(EndpointsEolConstantes.URL_FUNCIONARIO_EXTERNO_POR_CPF.Parametros(cpf));
            if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
                return null;
            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<FuncionarioExternoServicoEol[]>();
        }

        public async Task<IEnumerable<ComponenteCurricularAnoTurmaServicoEol>> ObterComponentesCurricularesEAnosTurmaPorAnoLetivo(int anoLetivo)
        {
            var resposta = await _httpClient.GetAsync(EndpointsEolConstantes.OBTER_COMPONENTE_CURRICULAR_E_ANO_TURMA_POR_ANO_LETIVO.Parametros(anoLetivo));

            if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
                throw new NegocioException(MensagemNegocio.NENHUM_COMPONENTE_CURRICULAR_DOS_ANOS_DA_TURMA_DO_EOL_FORAM_LOCALIZADOS, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<ComponenteCurricularAnoTurmaServicoEol[]>();
        }

        public async Task<IEnumerable<CursistaCargoServicoEol>> ObterCargosFuncionadoPorRegistroFuncional(string registroFuncional)
        {
            var resposta = await _httpClient.GetAsync(EndpointsEolConstantes.OBTER_CARGOS_FUNCIONARIO_POR_RF.Parametros(registroFuncional));

            if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
                throw new NegocioException(MensagemNegocio.ERRO_OBTER_CARGOS_FUNCIONARIO_EOL, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<CursistaCargoServicoEol[]>();
        }

        public async Task<IEnumerable<CursistaServicoEol>> ObterFuncionariosPorCargosFuncoesModalidadeAnosComponentesDres(IEnumerable<long> codigosCargos, IEnumerable<long> codigosFuncoes,
            IEnumerable<long> codigosModalidades, IEnumerable<string> anosTurma, IEnumerable<string> codigosDres, IEnumerable<long> codigosComponentesCurriculares, bool EhTipoJornadaJEIF)
        {
            var filtrosUrl = "?";

            if (codigosCargos.PossuiElementos())
                filtrosUrl += string.Join("", codigosCargos.Select(s => $"&CodigosCargos={s}"));

            if (codigosFuncoes.PossuiElementos())
                filtrosUrl += string.Join("", codigosFuncoes.Select(s => $"&CodigosFuncoes={s}"));

            if (anosTurma.PossuiElementos())
                filtrosUrl += string.Join("", anosTurma.Select(s => $"&AnosTurma={s}"));

            if (codigosComponentesCurriculares.PossuiElementos())
                filtrosUrl += string.Join("", codigosComponentesCurriculares.Select(s => $"&CodigosComponentesCurriculares={s}"));

            if (codigosModalidades.PossuiElementos())
                filtrosUrl += string.Join("", codigosModalidades.Select(s => $"&CodigoModalidade={s}"));

            if (codigosDres.PossuiElementos())
                filtrosUrl += string.Join("", codigosDres.Select(s => $"&CodigosDres={s}"));

            filtrosUrl += $"&EhTipoJornadaJEIF={EhTipoJornadaJEIF}";

            var resposta = await _httpClient.GetAsync(EndpointsEolConstantes.URL_FUNCIONARIOS_REGISTROS_FUNCIONAIS_CONECTA_FORMACAO + filtrosUrl);

            if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
                throw new NegocioException(MensagemNegocio.ERRO_OBTER_FUNCIONARIO_POR_CARGO_FUNCAO_ANO_MODALIDADE_COMPONENTE_EOL, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<CursistaServicoEol[]>();
        }

        public async Task<IEnumerable<DreUeAtribuicaoServicoEol>> ObterDreUeAtribuicaoPorFuncionarioCargo(string registroFuncional, long codigoCargo)
        {
            var resposta = await _httpClient.GetAsync(EndpointsEolConstantes.OBTER_DRE_UE_ATRIBUICAO_POR_FUNCIONARIO_CARGO.Parametros(registroFuncional, codigoCargo));

            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException(MensagemNegocio.ERRO_OBTER_DRE_UE_ATRIBUICAO_POR_FUNCIONARIO_E_CARGO_EOL, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<DreUeAtribuicaoServicoEol[]>();
        }

        public async Task<string> ObterNomeServidorPorRfEol(string rfServidor)
        {
            var resposta = await _httpClient.GetAsync(EndpointsEolConstantes.OBTER_NOME_SERVIDOR_EOL.Parametros(rfServidor));

            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException(MensagemNegocio.ERRO_OBTER_NOME_SERVIDOR_POR_RF_EOL, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<string>();
        }

        public async Task<IEnumerable<UsuarioPerfilServicoEol>> ObterUsuariosPorPerfis(IEnumerable<Guid> perfis)
        {
            var parametros = perfis.ObjetoParaJson();
            var resposta = await _httpClient.PostAsync(EndpointsEolConstantes.OBTER_USUARIOS_POR_PERFIS, new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));
            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException(MensagemNegocio.ERRO_OBTER_USUARIOS_POR_PERFIS, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<UsuarioPerfilServicoEol[]>();
        }

        public async Task<IEnumerable<string>> VerificarSeUsuarioEstaAtivo(string[] rf)
        {
            var parametros = new { codigosRfs = rf }.ObjetoParaJson();
            var resposta = await _httpClient.PostAsync(EndpointsEolConstantes.VERIFICAR_SE_FUNCIONARIOS_ESTAO_ATIVOS, new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));
            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException($"{MensagemNegocio.ERRO_A0_VERIFICAR_USUARIO_ATIVO} - {resposta.StatusCode}", resposta.StatusCode);
            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<IEnumerable<string>>();
        }
    }
}