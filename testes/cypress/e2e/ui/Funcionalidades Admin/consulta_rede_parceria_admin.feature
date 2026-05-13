# language: pt

Funcionalidade: Consulta de usuários de rede parceria

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin" 

  Esquema do Cenário: Validar consulta: <caso>
    E visualizo a tela Rede de Parceria
    Quando clico na Listagem de usuários "<situacao>"
    Então sistema consulta o usuário de rede de parceria

  Exemplos:
    | situacao | caso            |
    | Ativo    | Usuário ativo   |
    | Inativo  | Usuário inativo |

  Esquema do Cenário: Validar preenchimento do: <caso>
    E visualizo a tela Rede de Parceria
    Quando clico para consultar em Listagem de usuários de parceria "<situacao>"
    Então o sistema exibe o "<campo>" preenchido na tela do usuário de rede de parceria

    Exemplos:
      | situacao | campo          | caso                     |
      | Ativo    | area_promotora | Campo de área promotora  |
      | Ativo    | cpf            | Campo de CPF             |
      | Ativo    | nome_usuario   | Campo de nome do usuário |
      | Ativo    | email          | Campo de e-mail          |
      | Ativo    | telefone       | Campo de telefone        |
      | Ativo    | situacao       | Campo situação          |

  Esquema do Cenário: Validar filtro: <caso>
    E visualizo a tela Rede de Parceria
    Quando possuo cadastro em Listagem de usuários de parceria "<situacao>"
    Então sistema filtra o usuário de rede de parceria

  Exemplos:
    | situacao | caso               |
    | Ativo    | Usuário cadastrado |
