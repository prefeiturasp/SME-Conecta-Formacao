using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Constante;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Options;
using System.Net;
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
            var resposta = await _httpClient.PostAsync(EndpointsServicoAcessosConstantes.URL_AUTENTICACAO_AUTENTICAR, new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (resposta.StatusCode == HttpStatusCode.InternalServerError || resposta.StatusCode == HttpStatusCode.BadGateway || resposta.StatusCode == HttpStatusCode.ServiceUnavailable || resposta.StatusCode == HttpStatusCode.GatewayTimeout)
                throw new NegocioException(MensagemNegocio.SERVICO_AUTENTICACAO_FORA);

            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<AcessosUsuarioAutenticacaoRetorno>();
        }

        public async Task<AcessosPerfisUsuarioRetorno> RevalidarToken(string token)
        {
            var parametros = new { token }.ObjetoParaJson();
            var resposta = await _httpClient.PostAsync(EndpointsServicoAcessosConstantes.URL_AUTENTICACAO_REVALIDAR, new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException(MensagemNegocio.TOKEN_INVALIDO, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<AcessosPerfisUsuarioRetorno>();
        }

        public async Task<string> ObterLoginUsuarioToken(Guid token, TipoAcao tipoAcao)
        {
            var resposta = await _httpClient.GetAsync(string.Format(EndpointsServicoAcessosConstantes.URL_USUARIOS_X_SISTEMAS_Y_VALIDAR_Z, token, _servicoAcessosOptions.CodigoSistema, tipoAcao));

            if (!resposta.IsSuccessStatusCode) return string.Empty;

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<string>();
        }

        public async Task<AcessosPerfisUsuarioRetorno> ObterPerfisUsuario(string login)
        {
            var resposta = await _httpClient.GetAsync(string.Format(EndpointsServicoAcessosConstantes.URL_AUTENTICACAO_USUARIOS_X_SISTEMAS_Y_PERFIS, login, _servicoAcessosOptions.CodigoSistema));

            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException(MensagemNegocio.PERFIS_DO_USUARIO_NAO_LOCALIZADOS_VERIFIQUE_O_LOGIN, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<AcessosPerfisUsuarioRetorno>();
        }

        public async Task<AcessosPerfisUsuarioRetorno> ObterPerfisUsuario(string login, Guid perfilUsuarioId)
        {
            var resposta = await _httpClient.GetAsync(string.Format(EndpointsServicoAcessosConstantes.URL_AUTENTICACAO_USUARIOS_X_SISTEMAS_Y_PERFIS_Z, login, _servicoAcessosOptions.CodigoSistema, perfilUsuarioId));

            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException(MensagemNegocio.PERFIS_DO_USUARIO_NAO_LOCALIZADOS_VERIFIQUE_O_LOGIN, resposta.StatusCode);

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<AcessosPerfisUsuarioRetorno>();
        }

        public async Task<bool> UsuarioCadastradoCoreSSO(string login)
        {
            var resposta = await _httpClient.GetAsync(string.Format(EndpointsServicoAcessosConstantes.URL_USUARIOS_X_CADASTRADO, login));

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<bool>();
        }

        public async Task<bool> CadastrarUsuarioCoreSSO(string login, string nome, string email, string senha)
        {
            var parametros = new { login, nome, email, senha }.ObjetoParaJson();
            var resposta = await _httpClient.PostAsync(EndpointsServicoAcessosConstantes.URL_USUARIOS_CADASTRAR, new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<bool>();
        }

        public async Task<bool> AtualizarUsuarioCoreSSO(string login, string nome, string email, string senha)
        {
            var parametros = new { login, nome, email, senha }.ObjetoParaJson();
            var resposta = await _httpClient.PutAsync(EndpointsServicoAcessosConstantes.URL_USUARIOS_ATUALIZAR.Parametros(login), new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<bool>();
        }

        public async Task<bool> VincularPerfilExternoCoreSSO(string login, Guid perfilId)
        {
            var resposta = await _httpClient.PostAsync(string.Format(EndpointsServicoAcessosConstantes.URL_USUARIOS_X_VINCULAR_PERFIL_Y, login, perfilId), null);

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<bool>();
        }

        public async Task<AcessosDadosUsuario> ObterMeusDados(string login)
        {
            var resposta = await _httpClient.GetAsync(string.Format(EndpointsServicoAcessosConstantes.URL_USUARIOS_X, login));

            if (!resposta.IsSuccessStatusCode) return new AcessosDadosUsuario();

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<AcessosDadosUsuario>();
        }

        public Task<bool> AlterarSenha(string login, string senhaAtual, string senhaNova)
        {
            var json = new { login, senhaAtual, senhaNova, sistemaId = _servicoAcessosOptions.CodigoSistema }.ObjetoParaJson();
            return InvocarPutServicoAcessosRetornandoBool(string.Format(EndpointsServicoAcessosConstantes.URL_USUARIOS_X_SENHA, login), json);
        }

        public Task<bool> AlterarEmail(string login, string email)
        {
            var json = new { login, email }.ObjetoParaJson();
            return InvocarPutServicoAcessosRetornandoBool(string.Format(EndpointsServicoAcessosConstantes.URL_USUARIOS_X_EMAIL, login), json);
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
            var resposta = await _httpClient.GetAsync(string.Format(EndpointsServicoAcessosConstantes.URL_USUARIOS_X_SISTEMAS_Y_RECUPERAR_SENHA, login, _servicoAcessosOptions.CodigoSistema));

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

        public async Task<bool> ValidarUsuarioToken(Guid token)
        {
            var resposta = await _httpClient.GetAsync(string.Format(EndpointsServicoAcessosConstantes.URL_USUARIOS_X_SISTEMAS_Y_VALIDAR, token, _servicoAcessosOptions.CodigoSistema));

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<bool>();
        }

        public async Task<string> AlterarSenhaComTokenRecuperacao(Guid token, string novaSenha)
        {
            var parametros = new { token, senha = novaSenha }.ObjetoParaJson();
            var resposta = await _httpClient.PutAsync(string.Format(EndpointsServicoAcessosConstantes.URL_USUARIOS_SISTEMAS_X_SENHA, _servicoAcessosOptions.CodigoSistema), new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

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
            var resposta = await _httpClient.GetAsync(string.Format(EndpointsServicoAcessosConstantes.URL_GRUPOS_SISTEMA_X, _servicoAcessosOptions.CodigoSistema));

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
            var resposta = await _httpClient.GetAsync(string.Format(EndpointsServicoAcessosConstantes.URL_GRUPOS_SISTEMA_X_Y, _servicoAcessosOptions.CodigoSistema, grupoId));

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
        public async Task<bool> EnviarEmailValidacaoUsuarioExterno(string login)
        {
            var resposta = await _httpClient.PostAsync(string.Format(EndpointsServicoAcessosConstantes.URL_USUARIOS_X_SISTEMAS_Y_ENVIAR_EMAIL_VALIDACAO, login, _servicoAcessosOptions.CodigoSistema), null);

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return json.JsonParaObjeto<bool>();
        }

        public Task<bool> AlterarNome(string login, string nome)
        {
            var json = new { login, nome }.ObjetoParaJson();
            return InvocarPutServicoAcessosRetornandoBool(string.Format(EndpointsServicoAcessosConstantes.URL_USUARIOS_X_NOME, login), json);
        }

        public async Task<AcessosConfiguracaoEmailRetorno> ObterConfiguracaoEmail()
        {
            var resposta = await _httpClient.GetAsync(string.Format(EndpointsServicoAcessosConstantes.URL_CONFIGURACAO_EMAIL_SISTEMA_X, _servicoAcessosOptions.CodigoSistema));

            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                return json.JsonParaObjeto<AcessosConfiguracaoEmailRetorno>();
            }

            var mensagem = await resposta.Content.ReadAsStringAsync();
            throw new NegocioException(mensagem.JsonParaObjeto<string>());
        }

        public async Task<IEnumerable<RetornoUsuriosPareceristas>> ObterUsuariosPerfilPareceristas()
        {
            var resposta =
                await _httpClient.GetAsync(
                    EndpointsServicoAcessosConstantes.URL_OBTER_USUARIOS_PARECERISTAS);

            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                return json.JsonParaObjeto<IEnumerable<RetornoUsuriosPareceristas>>();
            }
            else
            {
                var mensagem = await resposta.Content.ReadAsStringAsync();
                throw new NegocioException(mensagem.JsonParaObjeto<string>());
            }
        }
    }
}
