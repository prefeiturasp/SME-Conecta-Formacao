using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Options;
using System.Text;

namespace SME.ConectaFormacao.Infra.Servicos.Acessos
{
    public class ServicoAcessos : IServicoAcessos
    {
        private readonly HttpClient _httpClient;
        private readonly ServicoAcessosOptions _servicoAcessosOptions;

        public ServicoAcessos(HttpClient httpClient, ServicoAcessosOptions servicoAcessosOptions)
        {
            this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this._servicoAcessosOptions = servicoAcessosOptions ?? throw new ArgumentNullException(nameof(servicoAcessosOptions));
        }

        public async Task<AcessosUsuarioAutenticacaoRetorno> Autenticar(string login, string senha)
        {
            var parametros = new { login, senha }.ObjetoParaJson();
            var resposta = await _httpClient.PostAsync($"v1/autenticacao/autenticar", new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<AcessosUsuarioAutenticacaoRetorno>();
        }

        public async Task<AcessosPerfisUsuarioRetorno> ObterPerfisUsuario(string login)
        {
            var resposta = await _httpClient.GetAsync($"v1/autenticacao/usuarios/{login}/sistemas/{_servicoAcessosOptions.CodigoSistema}/perfis");

            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException(MensagemNegocio.PERFIS_DO_USUARIO_NAO_LOCALIZADOS_VERIFIQUE_O_LOGIN, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<AcessosPerfisUsuarioRetorno>();
        }

        public async Task<AcessosPerfisUsuarioRetorno> ObterPerfisUsuario(string login, Guid perfilUsuarioId)
        {
            var resposta = await _httpClient.GetAsync($"v1/autenticacao/usuarios/{login}/sistemas/{_servicoAcessosOptions.CodigoSistema}/perfis/{perfilUsuarioId}");

            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException(MensagemNegocio.PERFIS_DO_USUARIO_NAO_LOCALIZADOS_VERIFIQUE_O_LOGIN, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<AcessosPerfisUsuarioRetorno>();
        }

        public async Task<bool> UsuarioCadastradoCoreSSO(string login)
        {
            var resposta = await _httpClient.GetAsync($"v1/usuarios/{login}/cadastrado");

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<bool>();
        }

        public async Task<bool> CadastrarUsuarioCoreSSO(string login, string nome, string email, string senha)
        {
            var parametros = new { login, nome, email, senha }.ObjetoParaJson();
            var resposta = await _httpClient.PostAsync($"v1/usuarios/cadastrar", new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<bool>();
        }

        public async Task<bool> VincularPerfilExternoCoreSSO(string login, Guid perfilId)
        {
            var resposta = await _httpClient.PostAsync($"v1/usuarios/{login}/vincular-perfil/{perfilId}", null);

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<bool>();
        }

        public async Task<AcessosDadosUsuario> ObterMeusDados(string login)
        {
            var resposta = await _httpClient.GetAsync($"v1/usuarios/{login}");

            if (!resposta.IsSuccessStatusCode) return new AcessosDadosUsuario();

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<AcessosDadosUsuario>();
        }

        public Task<bool> AlterarSenha(string login, string senhaAtual, string senhaNova)
        {
            var json = new { login, senhaAtual, senhaNova, sistemaId = _servicoAcessosOptions.CodigoSistema }.ObjetoParaJson();
            return InvocarPutServicoAcessosRetornandoBool($"v1/usuarios/{login}/senha", json);
        }

        public Task<bool> AlterarEmail(string login, string email)
        {
            var json = new { login, email }.ObjetoParaJson();
            return InvocarPutServicoAcessosRetornandoBool($"v1/usuarios/{login}/email", json);
        }

        private async Task<bool> InvocarPutServicoAcessosRetornandoBool(string rota, string parametros)
        {
            var resposta = await _httpClient.PutAsync(rota, new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<bool>();
        }

        public async Task<string> SolicitarRecuperacaoSenha(string login)
        {
            var resposta = await _httpClient.GetAsync($"v1/usuarios/{login}/sistemas/{_servicoAcessosOptions.CodigoSistema}/recuperar-senha");

            if (resposta.IsSuccessStatusCode)
            {
                var mensagem = await resposta.Content.ReadAsStringAsync();
                return mensagem.JsonParaObjeto<string>();
            }
            else
            {
                var mensagem = await resposta.Content.ReadAsStringAsync();
                throw new NegocioException(mensagem.JsonParaObjeto<string>());
            }
        }

        public async Task<bool> TokenRecuperacaoSenhaEstaValido(Guid token)
        {
            var resposta = await _httpClient.GetAsync($"v1/usuarios/{token}/sistemas/{_servicoAcessosOptions.CodigoSistema}/validar");

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<bool>();
        }

        public async Task<string> AlterarSenhaComTokenRecuperacao(Guid token, string novaSenha)
        {
            var parametros = new { token, senha = novaSenha }.ObjetoParaJson();
            var resposta = await _httpClient.PutAsync($"v1/usuarios/sistemas/{_servicoAcessosOptions.CodigoSistema}/senha", new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                return json.JsonParaObjeto<string>();
            }
            else
            {
                var mensagem = await resposta.Content.ReadAsStringAsync();
                throw new NegocioException(mensagem.JsonParaObjeto<string>());
            }
        }

        public async Task<IEnumerable<AcessosGrupo>> ObterGrupos()
        {
            var resposta = await _httpClient.GetAsync($"v1/grupos/sistema/{_servicoAcessosOptions.CodigoSistema}");

            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                return json.JsonParaObjeto<IEnumerable<AcessosGrupo>>();
            }
            else
            {
                var mensagem = await resposta.Content.ReadAsStringAsync();
                throw new NegocioException(mensagem.JsonParaObjeto<string>());
            }
        }
        public async Task<AcessosGrupo> ObterGrupoPorId(Guid grupoId)
        {
            var resposta = await _httpClient.GetAsync($"v1/grupos/sistema/{_servicoAcessosOptions.CodigoSistema}/{grupoId}");

            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                return json.JsonParaObjeto<AcessosGrupo>();
            }
            else
            {
                var mensagem = await resposta.Content.ReadAsStringAsync();
                throw new NegocioException(mensagem.JsonParaObjeto<string>());
            }
        }
    }
}
