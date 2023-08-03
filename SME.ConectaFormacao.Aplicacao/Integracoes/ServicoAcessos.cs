using Newtonsoft.Json;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Integracoes.Interfaces;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Integracoes
{
    public class ServicoAcessos : IServicoAcessos
    {
        private readonly HttpClient httpClient;
        private const int CODIGO_SISTEMA = 0000;

        public ServicoAcessos(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<UsuarioAutenticacaoRetornoDTO> Autenticar(string login, string senha)
        {
            var parametros = JsonConvert.SerializeObject(new { login, senha });
            var resposta = await httpClient.PostAsync($"v1/autenticacao/autenticar", new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS);

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UsuarioAutenticacaoRetornoDTO>(json);
        }

        public async Task<RetornoPerfilUsuarioDTO> ObterPerfisUsuario(string login)
        {
            var resposta = await httpClient.GetAsync($"v1/autenticacao/usuarios/{login}/sistemas/{CODIGO_SISTEMA}/perfis");

            if (!resposta.IsSuccessStatusCode)
                throw new NegocioException(MensagemNegocio.PERFIS_DO_USUARIO_NAO_LOCALIZADOS_VERIFIQUE_O_LOGIN);

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RetornoPerfilUsuarioDTO>(json);
        }

        public async Task<bool> UsuarioCadastradoCoreSSO(string login)
        {
            var resposta = await httpClient.GetAsync($"v1/usuarios/{login}/cadastrado");

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public async Task<bool> CadastrarUsuarioCoreSSO(string login, string nome, string email, string senha)
        {
            var parametros = JsonConvert.SerializeObject(new { login, nome, email, senha });
            var resposta = await httpClient.PostAsync($"v1/usuarios/cadastrar", new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public async Task<bool> VincularPerfilExternoCoreSSO(string login, Guid perfilId)
        {
            var resposta = await httpClient.PostAsync($"v1/usuarios/{login}/vincular-perfil/{perfilId}", null);

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public async Task<DadosUsuarioDTO> ObterMeusDados(string login)
        {
            var resposta = await httpClient.GetAsync($"v1/usuarios/{login}");

            if (!resposta.IsSuccessStatusCode) return new DadosUsuarioDTO();

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DadosUsuarioDTO>(json);
        }

        public Task<bool> AlterarSenha(string login, string senhaAtual, string senhaNova)
        {
            return InvocarPutServicoAcessosRetornandoBool($"v1/usuarios/{login}/senha",
                JsonConvert.SerializeObject(new { login, senhaAtual, senhaNova, sistemaId = CODIGO_SISTEMA }));
        }

        public Task<bool> AlterarEmail(string login, string email)
        {
            return InvocarPutServicoAcessosRetornandoBool($"v1/usuarios/{login}/email", JsonConvert.SerializeObject(new { login, email }));
        }

        private async Task<bool> InvocarPutServicoAcessosRetornandoBool(string rota, string parametros)
        {
            var resposta = await httpClient.PutAsync(rota, new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            var retorno = JsonConvert.DeserializeObject<bool>(json);
            return retorno;
        }

        public async Task<string> SolicitarRecuperacaoSenha(string login)
        {
            var resposta = await httpClient.GetAsync($"v1/usuarios/{login}/sistemas/{CODIGO_SISTEMA}/recuperar-senha");

            if (!resposta.IsSuccessStatusCode) return string.Empty;

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(json);
        }

        public async Task<bool> TokenRecuperacaoSenhaEstaValido(Guid token)
        {
            var resposta = await httpClient.GetAsync($"v1/usuarios/{token}/sistemas/{CODIGO_SISTEMA}/validar");

            if (!resposta.IsSuccessStatusCode) return false;

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public async Task<string> AlterarSenhaComTokenRecuperacao(RecuperacaoSenhaDto recuperacaoSenhaDto)
        {
            var parametros = JsonConvert.SerializeObject(new { token = recuperacaoSenhaDto.Token, senha = recuperacaoSenhaDto.NovaSenha });

            var resposta = await httpClient.PutAsync($"v1/usuarios/sistemas/{CODIGO_SISTEMA}/senha", new StringContent(parametros, Encoding.UTF8, "application/json-patch+json"));

            if (!resposta.IsSuccessStatusCode) return string.Empty;

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(json);
        }
    }
}
