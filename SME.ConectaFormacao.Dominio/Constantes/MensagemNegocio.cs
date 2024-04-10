namespace SME.ConectaFormacao.Dominio.Constantes;

public class MensagemNegocio
{
    public const string IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA = "Importação arquivo registro não localizada";
    public const string IMPORTACAO_ARQUIVO_NAO_LOCALIZADA = "Importação arquivo não localizada";
    public const string PARAMETRO_X_NAO_ENCONTRADO_PARA_ANO_Y = "Parâmetro '{0}' não encontrado para o ano '{1}'.";
    public const string A_PLANILHA_DE_INSCRICAO_CURSISTA_NAO_TEM_O_NOME_DA_COLUNA_Y_NA_COLUNA_Z = "A planilha de inscrição de cursistas deveria apresentar o nome '{0}' na coluna '{1}', conforme previsto planilha modelo.";
    public const string ARQUIVO_IMPORTADO_COM_SUCESSO = "Arquivo importado com sucesso";
    public const string CONTENT_TYPE_EXCEL = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public const string SOMENTE_ARQUIVO_XLSX_SUPORTADO = "Esse arquivo não é um XLSX. Somente arquivos do tipo XLSX são permitidos";
    public const string INSCRICAO_CONFIRMADA_NA_DATA_INICIO_DA_SUA_TURMA = "Sua inscrição foi confirmada. Na data de início da sua turma acesse o SGA para iniciar a formação.";
    public const string INSCRICAO_CONFIRMADA = "Sua inscrição foi confirmada.";
    public const string PARAMETRO_QTDE_CURSISTAS_SUPORTADOS_POR_TURMA_NAO_ENCONTRADO = "Parâmetro de QtdeCursistasSuportadosPorTurma ausente para o ano {0}.";
    public const string FORMACAO_NAO_ENCONTRADA = "Formação não encontrada";
    public const string PERIODO_REALIZACAO_NAO_INFORMADO = "É necessário informar as datas do período de realização";
    public const string PERIODO_INCRICAO_NAO_INFORMADO = "É necessário informar as datas do período de inscrição";
    public const string TIPO_FORMACAO_NAO_INFORMADO = "É necessário informar o tipo da formação";
    public const string FORMATO_NAO_INFORMADO = "É necessário informar o formato";
    public const string TIPO_INSCRICAO_NAO_INFORMADA = "É necessário informar o tipo da inscrição";
    public const string NOME_FORMACAO_NAO_INFORMADO = "É necessário informar o nome da formação";
    public const string PUBLICO_ALVO_NAO_INFORMADO = "É necessário informar o público alvo";
    public const string QUANTIDADE_DE_TURMAS_NAO_INFORMADA = "É necessário informar a quantidade de turmas";
    public const string QUANTIDADE_DE_VAGAS_POR_TURMAS_NAO_INFORMADA = "É necessário informar a quantidade de vagas por turma";
    public const string CRITERIOS_PARA_CERTIFICACAO_NAO_INFORMADO = "É necessário informar ao menos 3 critérios para certificação";
    public const string ACAO_INFORMATIVA_NAO_ACEITA = "É necessário aceitar a ação formativa";
    public const string DESCRICAO_DA_CERTIFICACAO_NAO_INFORMADA = "É necessario informar descrição da atividade para certificação";
    public const string QUANTIDADE_DE_VAGAS_SGA_MAIOR_QUE_O_PERMINTIDO = "A quantidade de vagas informada é maior que o permitido por turma no SGA(máximo permitido é {0})";

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
    public const string SERVICO_AUTENTICACAO_FORA = "Falha ao tentar se autenticar no servidor";
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
    public const string AREA_PROMOTORA_POSSUI_PROPOSTA = "Área promotora já possui proposta cadastrada";
    public const string EMAIL_INVALIDO = "E-mail {0} é inválido";
    public const string CPF_COM_DIGITO_VERIFICADOR_INVALIDO = "CPF {0} é inválido";
    public const string PROPOSTA_NAO_ENCONTRADA = "Proposta não encontrada";
    public const string PROPOSTA_JEIF_COM_OUTROS = "Proposta com os Tipos Inscrição Automática e Automática (JEIF) não podem conter a Função específica outros";
    public const string TOKEN_INVALIDO = "Token inválido";

    public const string PROPOSTA_FUNCAO_ESPECIFICA_OUTROS = "É necessário informar função específicas outros da proposta";
    public const string PROPOSTA_CRITERIO_VALIDACAO_INSCRICAO_OUTROS = "É necessário informar critérios de validação das inscrições outros da proposta";
    public const string PROPOSTA_CRITERIO_VALIDACAO_PUBLICO_ALVO_ANO_TURMA_COMPONENTE_CURRICULAR = "É necessário informar Público Alvo ou Função Específica ou Modalidade, Ano da Turma e Componente Curricular da proposta";
    public const string PROPOSTA_PALAVRA_CHAVE = "É necessário informar palavras chaves da proposta";

    public const string ARQUIVO_VAZIO = "É necessário informar o arquivo a ser carregado";
    public const string ARQUIVO_NENHUM_ARQUIVO_ENCONTRADO = "Nenhum arquivo encontrado";
    public const string ARQUIVO_NAO_ENCONTRADO = "Arquivo não encontrado";
    public const string ARQUIVO_FISICO_NAO_ENCONTRADO = "Arquivo físico não encontrado";
    public const string ARQUIVO_MAIOR_QUE_10_MB = "O tamanho máximo permitido para o arquivo é de 10MB";
    public const string PROFISSIONAL_NAO_LOCALIZADO = "Não foi possível encontrar nenhum profissional com o RF informado";
    public const string PROFISSIONAL_NAO_LOCALIZADO_RF_INVALIDO = "É necessário informar um RF válido para obter o nome do profissional";
    public const string CODIGOS_DRE_NAO_LOCALIZADO = "Nenhum código de Dre foi encontrado";
    public const string UNIDADE_NAO_LOCALIZADA_POR_CODIGO = "Nenhuma Unidade foi localizada com o código informado";
    public const string CONTRATO_EXTERNO_NAO_LOCALIZADO_POR_CPF = "Nenhum contrato externo foi localizado com o cpf informado";
    public const string NENHUMA_DRE_ENCONTRADA_NO_EOL = "Não foi possível localizar as Dres no EOL para a sincronização instituicional";
    public const string PROPOSTA_NAO_ESTA_COMO_CADASTRADA = "Proposta deve estar com situação de cadastrada para ser enviada para validação";
    public const string QUANTIDADE_TURMAS_COM_REGENTE = "A quantidade de turmas informada deve ser a mesma de turmas com regentes";
    public const string NAO_EXISTE_NENHUM_TUTOR = "Não existe nenhum tutor cadastrado na Proposta";
    public const string JA_EXISTE_ESSA_TURMA_PARA_ESSE_TURTOR = "O Tutor {0} já possui a turma {1}";
    public const string JA_EXISTE_ESSA_TURMA_PARA_ESSE_REGENTE = "O Regente {0} já possui a turma {1}";
    public const string NENHUM_COMPONENTE_CURRICULAR_DOS_ANOS_DA_TURMA_DO_EOL_FORAM_LOCALIZADOS = "Não foi possível localizar nenhum componente curricular para os anos das turmas do EOL";
    public const string ERRO_OBTER_CARGOS_FUNCIONARIO_EOL = "Não foi possível localizar os cargos do funcionário no EOL";
    public const string ERRO_OBTER_FUNCIONARIO_POR_CARGO_FUNCAO_ANO_MODALIDADE_COMPONENTE_EOL = "Não foi possível localizar os funcionário no EOL com base nos critérios de cargo, função, ano, modalidade e componente curricular";
    public const string ERRO_OBTER_DRE_UE_ATRIBUICAO_POR_FUNCIONARIO_E_CARGO_EOL = "Não foi possível obter as dre com atribuicao do servidor e cargo no EOL";
    public const string ERRO_OBTER_USUARIOS_POR_PERFIS = "Não foi possível obter os usuários dos perfis informados";

    public const string TURMA_NAO_ENCONTRADA = "Turma não encontrada";
    public const string NENHUMA_TURMA_ENCONTRADA = "Nenhuma turma encontrada para a proposta";
    public const string USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA = "Sua lotação/local de trabalho não corresponde com a DRE desta turma, sendo assim, não será possível inserir sua inscrição.";
    public const string USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_INSCRICAO_MANUAL = "A lotação/local de trabalho do cursista não corresponde com a DRE promotora desta formação. Deseja continuar?";
    public const string USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_AUTOMATICO = "Sua lotação/local de trabalho não corresponde com a DRE desta turma, sendo assim, não será possível inserir sua inscrição. {0}";
    public const string USUARIO_JA_INSCRITO_NA_PROPOSTA = "Este cursista já está matriculado nesta formação. Confira mais detalhes na lista de inscrição dessa formação.";
    public const string USUARIO_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO = "Cargo/Função selecionado não definido no público alvo da formação, sendo assim, não será possível inserir a sua inscrição.";
    public const string CURSISTA_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO_INSCRICAO_MANUAL = "Este cursista não possui cargo compatível com o público alvo da formação, não será possível realizar a sua inscrição.";
    public const string INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA = "Não foi possível confirmar sua inscrição, a turma selecionada não possui mais vagas disponível.";
    public const string INSCRICAO_AUTOMATICA_NAO_CONFIRMADA_POR_FALTA_DE_VAGA = "Não foi possível confirmar sua inscrição automática, a turma selecionada não possui mais vagas disponível. {0}";
    public const string USUARIO_NAO_ENCONTRADO = "Usuário não encontrado";
    public const string NOME_USUARIO_NAO_PREENCHIDO = "Nome do usuário não preenchido";
    public const string CARGO_SOBREPOSTO_FUNCAO_ATIVIDADE_NAO_ENCONTRADO = "Informações de Cargo, CargoSobreposto e Função Atividade no EOL não foram encontradas";
    public const string EMAIL_FORA_DOMINIO_REDE_DIRETA = "É permitido somente e-mails com o domínio @SME ou @EDU.SME";
    public const string INSCRICAO_NAO_ENCONTRADA = "Inscrição não encontrada";
    public const string NENHUMA_TURMA_COM_VAGA_DISPONIVEL = "Nenhuma turma com vaga disponível encontrada";
    public const string INTEGRAR_NO_SGA_EH_OBRIGATORIO_QUANDO_AREA_PROMOTORA_DIRETA = "O campo 'Integrar no SGA' deve ser preenchido quando a área promotora for direta.";
    public const string USUARIO_NAO_INSCRITO_AUTOMATICAMENTE_NAO_POSSUI_PUBLICO_ALVO_NA_FORMACAO = "Cargo não definido no público alvo da formação, sendo assim, não será possível inserir a sua inscrição automática. {0}";
    public const string USUARIO_NAO_INSCRITO_AUTOMATICAMENTE_NAO_POSSUI_FUNCAO_ESPECIFICA_NA_FORMACAO = "Função não definido na Função específica da formação, sendo assim, não será possível inserir a sua inscrição automática. {0}";
    public const string DRE_NAO_INFORMADA_PARA_TODAS_AS_TURMAS = "É necessário informar pelo menos uma DRE para as turmas selecionadas ou opçao todas";
    public const string TODAS_AS_TURMAS_DEVEM_POSSUIR_DRE_OU_OPCAO_TODOS = "Todas as turmas devem possuir uma DRE selecionada ou a opção de todas";
    public const string DATAFIM_INSCRICAO_NAO_PODE_SER_MAIOR_QUE_DATAFIM_REALIZACAO = "A Data Inscrição fim não pode ser maior que a data Realização fim";
    public const string CONFIRMACAO_SENHA_DEVE_SER_IGUAL_A_SENHA = "Confirmação da senha: Deve ser igual a senha";
    public const string A_SENHA_DEVE_TER_NO_MÍNIMO_8_CARACTERES = "A senha deve conter no minimo 8 caracteres";
    public const string A_SENHA_DEVE_TER_NO_MÁXIMO_12_CARACTERES = "A senha deve conter no máximo 12 caracteres";
    public const string CPF_INVALIDO = "O CPF informado é inválido";
    public const string A_SENHA_NAO_PODE_CONTER_ESPACOS_EM_BRANCO = "A senhão não pode conter espaço em branco";
    public const string A_SENHA_DEVE_CONTER_SOMENTE = "A senha deve conter pelo menos 1 letra maiúscula, 1 minúscula, 1 número e/ou 1 caractere especial e não pode conter acentuação";
    public const string NAO_FOI_POSSIVEL_CADASTRAR_USUARIO_EXTERNO_NO_CORESSO = "Não foi possível cadastrar usuário externo no CoreSSO";
    public const string VOCE_JA_POSSUI_LOGIN_CONECTA = "Você já possui login no sistema. Caso tenha esquecido a senha, clique em 'esqueci a senha'";
    public const string VOCE_JA_POSSUI_LOGIN_CORESSO = "Você já possui login no sistema. Acesse informando seu RF ou CPF e senha dos Sistemas da SME";
    public const string USUARIO_NAO_VALIDOU_EMAIL = "Você não validou seu e-mail ainda. Caso não tenha recebido o e-mail clique no botão 'Reenviar'";
    public const string EMAIL_FORA_DOMINIO_PERMITIDO_UES_PARCEIRAS = "Endereço de email não está em um domínio permitido";

    public const string CURSISTA_NAO_ENCONTRADO = "Nenhum cursista foi encontrado";
  
    public const string PROPOSTA_COM_PUBLICO_ALVO_SEM_DEPARA_CONFIGURADO = "A Proposta {0} possui publico alvo sem o depara com eol configurado";
    public const string PROPOSTA_COM_FUNCAO_ESPECIFICA_SEM_DEPARA_CONFIGURADO = "A Proposta {0} possui função específica sem o depara com eol configurado";
    public const string TUTOR_JA_EXISTE_NA_PROPOSTA = "Já existe um tutor com o CPF informado para {0}";
    public const string REGENTE_JA_EXISTE_NA_PROPOSTA = "Já existe um regente com o CPF informado para {0}";
    public const string INFORME_O_CARGO = "É necessário informar o cargo ";

    public const string PROPOSTA_X_ALTERADA_COM_SUCESSO = "Proposta '{0}' alterada com sucesso!";
    public const string PROPOSTA_X_INSERIDA_COM_SUCESSO = "Proposta '{0}' inserida com sucesso!";
    public const string PROPOSTA_PUBLICADA_ALTERADA = "\nForam realizadas alterações de parâmetros de inscrição. Caso necessário cancele as inscrições com os parâmetros anteriores na tela de inscrições.";
    public const string PROPOSTA_PUBLICADA_ALTERADA_COM_INSCRICAO_AUTOMATICA = "\nAtenção: Não serão feitas novas inscrições automáticas para os novos parâmetros.";
    public const string VALIDAR_EMAIL_USUARIO_EXTERNO = "Cadastro inserido com sucesso. Enviamos um e-mail para validação do seu cadastro. Confira a sua caixa de entrada!";
    public const string USUARIO_EXTRNO_CADASTRADO_COM_SUCESSO = "Cadastro realizado com sucesso";
    public const string REALIZE_SEU_CADASTRO_NO_SISTEMA = "Usuário não cadastrado no sistema, realize o seu cadastro";

    public const string INSCRICAO_MANUAL_REALIZADA_COM_SUCESSO = "Inscrição manual realizada com sucesso";

    public const string SITUACAO_DO_ARQUIVO_DEVE_SER_VALIDADO = "A situação do arquivo deve ser validado";
    public const string USUARIO_NAO_FOI_ENCONTRADO_COM_O_REGISTRO_FUNCIONAL_OU_CPF_INFORMADOS = "O usuário não foi encontrado com o Registro Funcional ou CPF informados";
    public const string INSCRICAO_FORA_DO_PERIODO_INSCRICAO = "Inscrição fora do período de inscrição";
    public const string RF_MENOR_QUE_7_DIGITOS = "RF do arquivo foi preenchido com menos de 7 dígitos";

    public const string CARGO_NAO_ENCONTRATO_PARA_ALTERACAO_VINCULO_INSCRICAO = "O cargo não foi encontrado para alteração do vínculo da inscrição";
    public const string ATUALIZACAO_VINCULO_INSCRICAO_NAO_LOCALIZADA = "Atualização do vínculo da inscrição não localizada";
    public const string AS_INSCRICOES_PARA_ESTA_PROPOSTA_NAO_ESTAO_ABERTAS = "As inscrições para esta proposta não estão abertas.";

    public const string DADOS_ENVIO_EMAIL_NAO_LOCALIZADO = "Os dados para o envio do e-mail não foram localizados.";

    public const string EMAIL_AREA_PROMOTORA_NAO_CADASTRADO_ENVIO_EMAIL = "O e-mail da área promotora não foi cadastrado para envio do e-mail.";
}