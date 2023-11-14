namespace SME.ConectaFormacao.Dominio.Constantes;

public class MensagemNegocio
{
    public const string CAMPO_OBRIGATORIO_NAO_INFORMADO = "É necessário informar {0}";
    public const string PERFIS_DO_USUARIO_NAO_LOCALIZADOS_VERIFIQUE_O_LOGIN = "Os perfis do usuário não foram localizados! Verifique o usuário.";
    public const string USUARIO_OU_SENHA_INVALIDOS = "Usuário ou senha inválidos";
    public const string CONFIRMACAO_SENHA_INVALIDA = "Confirmação de senha não confere";
    public const string SENHA_NAO_ATENDE_CRITERIOS_SEGURANCA = "A senha não atendo os critérios de segurança: \r\nUma letra maiúscula \r\nUma letra minúscula \r\nUm algarismo (número) ou um símbolo (caractere especial) \r\nNão é permitido caracteres acentuados \r\nDeve ter no mínimo 8 e no máximo 12 caracteres.";
    public const string LOGIN_OU_SENHA_ATUAL_NAO_COMFEREM = "Usuário ou senha atual não conferem";
    public const string LOGIN_NAO_ENCONTRADO = "Usuário não encontrado";
    public const string ORIENTACOES_RECUPERACAO_SENHA = "As orientações para recuperação de senha foram enviados para {0}, verifique sua caixa de entrada!";
    public const string AREA_PROMOTORA_NAO_ENCONTRADA = "Área promotora não encontrada";
    public const string AREA_PROMOTORA_NAO_ENCONTRADA_GRUPO_USUARIO = "Nenhuma área promotora foi encontrada para o perfil do usuário logado";
    public const string AREA_PROMOTORA_EMAIL_FORA_DOMINIO_REDE_DIRETA = "Para área promotora do tipo rede direta é permitido somente e-mails com o domínio @SME ou @EDU.SME";
    public const string AREA_PROMOTORA_EXISTE_GRUPO_CADASTRADO = "Já existe uma área promotora cadastrada para o perfil selecionado";
    public const string AREA_PROMOTORA_EXISTE_GRUPO_DRE_CADASTRADO = "Já existe uma área promotora cadastrada para o perfil e dre selecionado";
    public const string EMAIL_INVALIDO = "E-mail {0} é inválido";
    public const string PROPOSTA_NAO_ENCONTRADA = "Proposta não encontrada";

    public const string PROPOSTA_FUNCAO_ESPECIFICA_OUTROS = "É necessário informar função específicas outros da proposta";
    public const string PROPOSTA_CRITERIO_VALIDACAO_INSCRICAO_OUTROS = "É necessário informar critérios de validação das inscrições outros da proposta";
    public const string PROPOSTA_PALAVRA_CHAVE = "É necessário informar palavras chaves da proposta";

    public const string ARQUIVO_VAZIO = "É necessário informar o arquivo a ser carregado";
    public const string ARQUIVO_NENHUM_ARQUIVO_ENCONTRADO = "Nenhum arquivo encontrado";
    public const string ARQUIVO_NAO_ENCONTRADO = "Arquivo não encontrado";
    public const string ARQUIVO_FISICO_NAO_ENCONTRADO = "Arquivo físico não encontrado";
    public const string ARQUIVO_MAIOR_QUE_10_MB = "O tamanho máximo permitido para o arquivo é de 10MB";
    public const string PROFISSIONAL_NAO_LOCALIZADO = "Não foi possível encontrar nenhum profissional com o RF informado";
    public const string PROFISSIONAL_NAO_LOCALIZADO_RF_INVALIDO = "É necessário informar um RF válido para obter o nome do profissional";
    public const string CODIGOS_DRE_NAO_LOCALIZADO = "Nenhum código de Dre foi encontrado";
    public const string NENHUMA_DRE_ENCONTRADA_NO_EOL = "Não foi possível localizar as Dres no EOL para a sincronização instituicional";
    public const string PROPOSTA_NAO_ESTA_COMO_CADASTRADA = "Proposta deve estar com situação de cadastrada para ser enviada para o DF";
    public const string NAO_EXISTE_NENHUM_REGENTE = "Não existe nenhum regente cadastrado na Proposta";
    public const string NAO_EXISTE_NENHUM_TUTOR = "Não existe nenhum tutor cadastrado na Proposta";
}