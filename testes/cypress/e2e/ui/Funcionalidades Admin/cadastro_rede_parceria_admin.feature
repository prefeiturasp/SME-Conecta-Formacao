# language: pt

Funcionalidade: Cadastro de usuários de rede parceria

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin" 

  Esquema do Cenário: Validar cadastro: <caso>
    E visualizo a tela Rede de Parceria
    Quando clico em "Novo" em Listagem de usuários para "<situacao>"
    Então sistema cadastra usuário de rede de parceria

  Exemplos:
    | situacao | caso                           |
    | Ativo    | Cadastrar usuário com sucesso  |
    | Inativo  | Cadastrar usuário como inativo |

  Esquema do Cenário: Validar campos obrigatórios: <caso>
    E visualizo a tela Rede de Parceria
    Quando clico em "Novo" em Listagem de usuários de parceria
    Então sistema não permite cadastrar usuários de parceria com campos obrigatórios vazios

  Exemplos:
    | tipo          | caso                |
    | Rede Parceria | Não permitir salvar |

  Esquema do Cenário: Validar exibição do: <caso>
    E visualizo a tela Rede de Parceria
    Quando clico em "Novo" em Listagem de usuários de parceria
    Então o sistema exibe o "<campo>" no cadastro de usuário de rede de parceria

    Exemplos:
      | campo          | caso                     |
      | area_promotora | Campo de área promotora  |
      | cpf            | Campo de CPF             |
      | nome_usuario   | Campo de nome do usuário |
      | email          | Campo de e-mail          |
      | telefone       | Campo de telefone        |
      | situacao       | Campoo situação          |

