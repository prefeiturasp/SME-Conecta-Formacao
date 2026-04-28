# language: pt

Funcionalidade: Edição de usuários de rede parceria

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"  

  Esquema do Cenário: Validar edição: <caso>
    E visualizo a tela Rede de Parceria
    Quando edito o usuário de parceria "<situacao>"
    Então sistema salva a alteração do usuário de parceria

  Exemplos:
    | situacao | caso                     |
    | Ativo    | Alterar dados do usuário |

  Esquema do Cenário: Validar cancelamento de edição: <caso>
    E visualizo a tela Rede de Parceria
    Quando cancelo a edição o usuário de parceria "<situacao>"
    Então sistema não salva a alteração do usuário de parceria

  Exemplos:
    | situacao | caso                         |
    | Ativo    | Não alterar dados do usuário |
