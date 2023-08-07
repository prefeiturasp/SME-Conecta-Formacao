using Newtonsoft.Json;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
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
            var parametros = JsonConvert.SerializeObject(new { login, senha });
            var resposta = await _httpClient.PostAsync($"v1/autenticacao/autenticar", new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS);

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AcessosUsuarioAutenticacaoRetorno>(json);
        }

        public async Task<AcessosRetornoPerfilUsuario> ObterPerfisUsuario(string login)
        {
            var resposta = await _httpClient.GetAsync($"v1/autenticacao/usuarios/{login}/sistemas/{_servicoAcessosOptions.CodigoSistema}/perfis");

            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException(MensagemNegocio.PERFIS_DO_USUARIO_NAO_LOCALIZADOS_VERIFIQUE_O_LOGIN);

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AcessosRetornoPerfilUsuario>(json);
        }

        public async Task<bool> UsuarioCadastradoCoreSSO(string login)
        {
            var resposta = await _httpClient.GetAsync($"v1/usuarios/{login}/cadastrado");

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public async Task<bool> CadastrarUsuarioCoreSSO(string login, string nome, string email, string senha)
        {
            var parametros = JsonConvert.SerializeObject(new { login, nome, email, senha });
            var resposta = await _httpClient.PostAsync($"v1/usuarios/cadastrar", new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public async Task<bool> VincularPerfilExternoCoreSSO(string login, Guid perfilId)
        {
            var resposta = await _httpClient.PostAsync($"v1/usuarios/{login}/vincular-perfil/{perfilId}", null);

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public async Task<AcessosDadosUsuario> ObterMeusDados(string login)
        {
            var resposta = await _httpClient.GetAsync($"v1/usuarios/{login}");

            if (!resposta.IsSuccessStatusCode) return new AcessosDadosUsuario();

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AcessosDadosUsuario>(json);
        }

        public Task<bool> AlterarSenha(string login, string senhaAtual, string senhaNova)
        {
            return InvocarPutServicoAcessosRetornandoBool($"v1/usuarios/{login}/senha",
                JsonConvert.SerializeObject(new { login, senhaAtual, senhaNova, sistemaId = _servicoAcessosOptions.CodigoSistema }));
        }

        public Task<bool> AlterarEmail(string login, string email)
        {
            return InvocarPutServicoAcessosRetornandoBool($"v1/usuarios/{login}/email", JsonConvert.SerializeObject(new { login, email }));
        }

        private async Task<bool> InvocarPutServicoAcessosRetornandoBool(string rota, string parametros)
        {
            var resposta = await _httpClient.PutAsync(rota, new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            var retorno = JsonConvert.DeserializeObject<bool>(json);
            return retorno;
        }

        public async Task<string> SolicitarRecuperacaoSenha(string login)
        {
            var resposta = await _httpClient.GetAsync($"v1/usuarios/{login}/sistemas/{_servicoAcessosOptions.CodigoSistema}/recuperar-senha");

            if (!resposta.IsSuccessStatusCode) return string.Empty;

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(json);
        }

        public async Task<bool> TokenRecuperacaoSenhaEstaValido(Guid token)
        {
            var resposta = await _httpClient.GetAsync($"v1/usuarios/{token}/sistemas/{_servicoAcessosOptions.CodigoSistema}/validar");

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public async Task<string> AlterarSenhaComTokenRecuperacao(AcessosRecuperacaoSenha recuperacaoSenhaDto)
        {
            var parametros = JsonConvert.SerializeObject(new { token = recuperacaoSenhaDto.Token, senha = recuperacaoSenhaDto.NovaSenha });

            var resposta = await _httpClient.PutAsync($"v1/usuarios/sistemas/{_servicoAcessosOptions.CodigoSistema}/senha", new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (!resposta.IsSuccessStatusCode) return string.Empty;

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(json);
        }
    }
}
