# language: pt

Funcionalidade: Redefinir senha de usuário admin

  Contexto:
    Validação dos cenários e campos obrigatórios
  
  Esquema do Cenário: Validar: <caso>
    Dado eu acesso o Conecta Formação com a visualização web
    E clico em "Esqueci minha senha"
    Quando clico em continuar com usuário "Admin"
    Então o sistema envia as orientações para recuperação de senha "<campo>"

    Exemplos:
      | campo   | caso                           |
      | usuário | Solicitar recuperação de senha |

  Esquema do Cenário: Validar: <caso>
    Dado eu acesso o Conecta Formação com a visualização web
    E clico em "Esqueci minha senha"
    Quando clico em continuar com usuário inválido
    Então o sistema informa "<campo>" inválido para recuperação de senha

    Exemplos:
      | campo   | caso                               |
      | usuário | Não solicita para usuário inválido |

  Esquema do Cenário: Validar: <caso>
    Dado eu acesso o Conecta Formação com a visualização web
    E clico em "Esqueci minha senha"
    Quando clico em continuar com usuário menor que o válido
    Então o sistema informa "<campo>" não contém o mínimo 5 caracteres

    Exemplos:
      | campo   | caso                                       |
      | usuário | Usuário deve conter no mínimo 5 caracteres |

  Esquema do Cenário: Validar: <caso>
    Dado eu acesso o Conecta Formação com a visualização web   
    Quando acesso com link expirado para alterar senha
    Então o sistema informa para solicitar novamente redefinição

    Exemplos:
      | caso                                     |
      | Não permitir redefinir com link expirado |

  Esquema do Cenário: Validar: <caso>
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"
    E acesso o menu Meus Dados
    Quando clico em alterar "senha" nos meus dados
    Então o campo "<campo>" do modal de senha deve estar visível

    Exemplos:
      | campo             | caso                       |
      | nova senha        | Preencimento da nova senha |
      | confirmação senha | Confirmação da nova senha  |
  
  Esquema do Cenário: Validar: <caso>
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"
    E acesso o menu Meus Dados
    Quando clico em alterar "senha" nos meus dados
    Então o campo "senha atual" do modal de senha deve estar visível
    E o campo "nova senha" do modal de senha deve estar visível
    E o campo "confirmação senha" do modal de senha deve estar visível
    Quando preencho o modal de senha com dados válidos
    Então realiza a alteração de senha

    Exemplos:
      | caso                          |
      | Alteração da senha do usuário |