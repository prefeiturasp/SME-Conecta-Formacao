# language: pt

Funcionalidade: Cadastro de Área Promotora

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin" 

  Esquema do Cenário: Validar cadastro: <caso>
    E visualizo a tela "Área Promotora"
    Quando clico em "Novo" em Cadastro da Área Promotora do tipo "<tipo>"
    Então sistema cadastra área promotora dos tipos

  Exemplos:
    | tipo          | caso                   |
    | Rede Parceria | Tipo rede parceria     |
    | Rede Direta   | Tipo rede direta       |

  Esquema do Cenário: Validar campos obrigatórios: <caso>
    E visualizo a tela "Área Promotora"
    Quando clico em "Novo" em Cadastro da Área Promotora
    Então sistema não permite cadastrar área promotora com campos obrigatórios vazios

  Exemplos:
    | tipo          | caso                |
    | Rede Parceria | Não permitir salvar |

