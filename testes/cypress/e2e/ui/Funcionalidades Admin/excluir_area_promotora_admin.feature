# language: pt

Funcionalidade: Excluir Área Promotora

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin" 

  Esquema do Cenário: Validar a: <caso>
    E visualizo a tela "Área Promotora"
    Quando clico no cadastro da Área Promotora
    E depois clico para excluir da promotora
    Então sistema confirma a exclusão de área promotora

  Exemplos:
    | caso                 |
    | Exclusão do cadastro |

  Esquema do Cenário: Validar cancelamento da ação: <caso>
    E visualizo a tela "Área Promotora"
    Quando clico no cadastro da Área Promotora
    E depois cancelo a exclusão da promotora
    Então sistema não confirma a exclusão de área promotora

  Exemplos:
    | caso             |
    | Excluir cadastro |


  Esquema do Cenário: Validar não permitir excluir: <caso>
    E visualizo a tela "Área Promotora"
    Quando clico no cadastro da Área Promotora
    E depois tento a exclusão da promotora
    Então sistema não exclui área promotora com proposta cadastrada

  Exemplos:
    | caso                    |
    | Com proposta cadastrada |
