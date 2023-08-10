namespace SME.ConectaFormacao.Dominio.Constantes;

public class MensagemNegocio
{
    public const string PERFIS_DO_USUARIO_NAO_LOCALIZADOS_VERIFIQUE_O_LOGIN = "Os perfis do usuário não foram localizados! Verifique o usuário.";
    public const string USUARIO_OU_SENHA_INVALIDOS = "Usuário ou senha inválidos";
    public const string CONFIRMACAO_SENHA_INVALIDA = "Confirmação de senha não confere";
    public static string SENHA_NAO_ATENDE_CRITERIOS_SEGURANCA = "A senha não atendo os critérios de segurança: \r\nUma letra maiúscula \r\nUma letra minúscula \r\nUm algarismo (número) ou um símbolo (caractere especial) \r\nNão é permitido caracteres acentuados \r\nDeve ter no mínimo 8 e no máximo 12 caracteres.";
    public const string LOGIN_OU_SENHA_ATUAL_NAO_COMFEREM = "Usuário ou senha atual não conferem";
}
