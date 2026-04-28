# language: pt

Funcionalidade: Exclusão de usuários de rede parceria

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin" 

  Esquema do Cenário: Validar exclusão de: <caso>
    E visualizo a tela Rede de Parceria
    Quando clico para excluir o usuário "<situacao>" da listagem
    Então sistema exclue o usuário de rede de parceria

  Exemplos:
    | situacao | caso            |
    | Ativo    | Usuário ativo   |
    | Inativo  | Usuário inativo |

  Esquema do Cenário: Validar cancelamento: <caso>
    E visualizo a tela Rede de Parceria
    Quando clico para cancelar a exclusão do usuário "<situacao>" 
    Então sistema não exclui usuário de rede de parceria

  Exemplos:
    | situacao | caso                        |
    | Ativo    | Não excluir usuário ativo   |
    | Inativo  | Não excluir usuário inativo |



