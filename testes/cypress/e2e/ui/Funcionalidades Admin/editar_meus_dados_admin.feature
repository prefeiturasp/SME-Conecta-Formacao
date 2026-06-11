# language: pt

Funcionalidade: Edição de Meus Dados

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"
    E acesso o menu Meus Dados

  Cenário: Validar alteração do nome no modal
    Quando clico em alterar "nome" nos meus dados
    Então o modal de alteração deve ser exibido

  Cenário: Validar alteração do email no modal
    Quando clico em alterar "email" nos meus dados
    Então o modal de alteração deve ser exibido

  Cenário: Validar alteração da senha no modal
    Quando clico em alterar "senha" nos meus dados
    Então o modal de alteração deve ser exibido

  Cenário: Validar cancelar a alteração
    Quando clico em alterar "nome" nos meus dados
    E clico em cancelar no modal de alteração
    Então o modal de alteração não deve estar visível

  Esquema do Cenário: Validar: <caso>
    Quando clico em alterar "senha" nos meus dados
    Então o campo "<campo>" do modal de senha deve estar visível

    Exemplos:
      | campo             | caso                   |
      | senha atual       | Senha atual            |
      | nova senha        | Inserir nova senha     |
      | confirmação senha | Confirmação nova senha |

  Cenário: Validar alteração de senha
    Quando clico em alterar "senha" nos meus dados
    Então o campo "senha atual" do modal de senha deve estar visível
    E o campo "nova senha" do modal de senha deve estar visível
    E o campo "confirmação senha" do modal de senha deve estar visível
    Quando preencho o modal de senha com dados válidos
    Então realiza a alteração de senha
  
  Cenário: Validar salvar meus dados
    Quando clico em alterar "nome" nos meus dados
    E clico em salvar no modal de alteração de dados
    Então mensagem de alteração dos meus dados deve ser exibida