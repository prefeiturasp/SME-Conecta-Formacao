namespace SME.ConectaFormacao.Dominio.Constantes;

public class MensagemNegocio
{
    public const string PERIODO_REALIZACAO_NAO_INFORMADO = "É necessário informar as datas do período de realização";
    public const string PERIODO_INCRICAO_NAO_INFORMADO = "É necessário informar as datas do período de inscrição";
    public const string TIPO_FORMACAO_NAO_INFORMADO = "É necessário informar o tipo da formação";
    public const string FORMATO_NAO_INFORMADO = "É necessário informar o formato";
    public const string TIPO_INSCRICAO_NAO_INFORMADA = "É necessário informar o tipo da inscrição";
    public const string NOME_FORMACAO_NAO_INFORMADO = "É necessário informar o nome da formação";
    public const string PUBLICO_ALVO_NAO_INFORMADO = "É necessário informar o público alvo";
    public const string QUANTIDADE_DE_TURMAS_NAO_INFORMADA = "É necessário informar a quantidade de turmas";
    public const string QUANTIDADE_DE__VAGAS_POR_TURMAS_NAO_INFORMADA = "É necessário informar a quantidade de vagas por turma";
    public const string CRITERIOS_PARA_CERTIFICACAO_NAO_INFORMADO = "É necessário informar ao menos 3 critérios para certificação";
    public const string ACAO_INFORMATIVA_NAO_ACEITA = "É necessário aceitar a ação formativa";
    public const string DESCRICAO_DA_CERTIFICACAO_NAO_INFORMADA = "É necessario informar descrição da atividade para certificação";

    public const string CARGA_HORARIA_NAO_INFORMADA = "É necessário informar a carga horária presencial";
    public const string JUSTIFICATIVA_NAO_INFORMADA = "É necessário informar a justificativa";
    public const string OBJETIVO_NAO_INFORMADO = "É necessário informar os objetivos";
    public const string CONTEUDO_PROGRAMATICO_NAO_INFORMADO = "É necessário informar o conteúdo programático";
    public const string PROCEDIMENTOS_METODOLOGICOS_NAO_INFORMADO = "É necessário informar os procedimentos metodológicos";
    public const string REFERENCIA_NAO_INFORMADA = "É necessário informar as referências";
    public const string PALAVRA_CHAVE_NAO_INFORMADA = "É necessário informar no mínimo 3 e no máximo 5 palavras chaves";

    public const string QUANTIDADE_TURMAS_COM_ENCONTRO_DIFERENTE_QUANTIDADE_DE_TURMAS = "A quantidade de turmas informada deve ser a mesma de turmas com encontros";
    public const string PERFIS_DO_USUARIO_NAO_LOCALIZADOS_VERIFIQUE_O_LOGIN = "Os perfis do usuário não foram localizados! Verifique o usuário.";
    public const string USUARIO_OU_SENHA_INVALIDOS = "Usuário ou senha inválidos";
    public const string CONFIRMACAO_SENHA_INVALIDA = "Confirmação de senha não confere";
    public const string SENHA_NAO_ATENDE_CRITERIOS_SEGURANCA = "A senha não atendo os critérios de segurança: \r\nUma letra maiúscula \r\nUma letra minúscula \r\nUm algarismo (número) ou um símbolo (caractere especial) \r\nNão é permitido caracteres acentuados \r\nDeve ter no mínimo 8 e no máximo 12 caracteres.";
    public const string LOGIN_OU_SENHA_ATUAL_NAO_CONFEREM = "Usuário ou senha atual não conferem";
    public const string LOGIN_NAO_ENCONTRADO = "Usuário não encontrado";
    public const string ORIENTACOES_RECUPERACAO_SENHA = "As orientações para recuperação de senha foram enviados para {0}, verifique sua caixa de entrada!";
    public const string AREA_PROMOTORA_NAO_ENCONTRADA = "Área promotora não encontrada";
    public const string AREA_PROMOTORA_NAO_ENCONTRADA_GRUPO_USUARIO = "Nenhuma área promotora foi encontrada para o perfil do usuário logado";
    public const string AREA_PROMOTORA_EMAIL_FORA_DOMINIO_REDE_DIRETA = "Para área promotora do tipo rede direta é permitido somente e-mails com o domínio @SME ou @EDU.SME";
    public const string AREA_PROMOTORA_EXISTE_GRUPO_CADASTRADO = "Já existe uma área promotora cadastrada para o perfil selecionado";
    public const string AREA_PROMOTORA_EXISTE_GRUPO_DRE_CADASTRADO = "Já existe uma área promotora cadastrada para o perfil e dre selecionado";
    public const string EMAIL_INVALIDO = "E-mail {0} é inválido";
    public const string PROPOSTA_NAO_ENCONTRADA = "Proposta não encontrada";
    public const string TOKEN_INVALIDO = "Token inválido";

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
    public const string PROPOSTA_NAO_ESTA_COMO_CADASTRADA = "Proposta deve estar com situação de cadastrada para ser enviada para validação";
    public const string QUANTIDADE_TURMAS_COM_REGENTE = "A quantidade de turmas informada deve ser a mesma de turmas com regentes";
    public const string NAO_EXISTE_NENHUM_TUTOR = "Não existe nenhum tutor cadastrado na Proposta";
    public const string JA_EXISTE_ESSA_TURMA_PARA_ESSE_TURTOR = "O Tutor {0} já possui a turma {1}";
    public const string JA_EXISTE_ESSA_TURMA_PARA_ESSE_REGENTE = "O Regente {0} já possui a turma {1}";
}