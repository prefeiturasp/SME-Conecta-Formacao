namespace SME.ConectaFormacao.Infra.Servicos.Acessos.Constante
{
    internal class EndpointsServicoAcessosConstantes
    {
        public const string URL_AUTENTICACAO_AUTENTICAR = "v1/autenticacao/autenticar";
        public const string URL_AUTENTICACAO_REVALIDAR = "v1/autenticacao/revalidar";
        public const string URL_AUTENTICACAO_USUARIOS_X_SISTEMAS_Y_PERFIS = "v1/autenticacao/usuarios/{0}/sistemas/{1}/perfis";
        public const string URL_AUTENTICACAO_USUARIOS_X_SISTEMAS_Y_PERFIS_Z = "v1/autenticacao/usuarios/{0}/sistemas/{1}/perfis/{2}";
        public const string URL_USUARIOS_X_CADASTRADO = "v1/usuarios/{0}/cadastrado";
        public const string URL_USUARIOS_ATUALIZAR = "v1/usuarios/{0}";
        public const string URL_USUARIOS_CADASTRAR = "v1/usuarios/cadastrar";
        public const string URL_USUARIOS_X_VINCULAR_PERFIL_Y = "v1/usuarios/{0}/vincular-perfil/{1}";
        public const string URL_USUARIOS_X = "v1/usuarios/{0}";
        public const string URL_USUARIOS_X_SENHA = "v1/usuarios/{0}/senha";
        public const string URL_USUARIOS_X_EMAIL = "v1/usuarios/{0}/email";
        public const string URL_USUARIOS_X_SISTEMAS_Y_RECUPERAR_SENHA = "v1/usuarios/{0}/sistemas/{1}/recuperar-senha";
        public const string URL_USUARIOS_X_SISTEMAS_Y_VALIDAR = "v1/usuarios/{0}/sistemas/{1}/validar";
        public const string URL_USUARIOS_X_SISTEMAS_Y_VALIDAR_Z = "v1/usuarios/{0}/sistemas/{1}/validar/{2}";
        public const string URL_USUARIOS_X_SISTEMAS_Y_ENVIAR_EMAIL_VALIDACAO = "v1/usuarios/{0}/sistemas/{1}/enviar-email-validacao";
        public const string URL_USUARIOS_SISTEMAS_X_SENHA = "v1/usuarios/sistemas/{0}/senha";
        public const string URL_GRUPOS_SISTEMA_X = "v1/grupos/sistema/{0}";
        public const string URL_GRUPOS_SISTEMA_X_Y = "v1/grupos/sistema/{0}/{1}";
    }
}
