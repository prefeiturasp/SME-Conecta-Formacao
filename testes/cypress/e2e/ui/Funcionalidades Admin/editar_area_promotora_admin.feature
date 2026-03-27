# language: pt

Funcionalidade: Editar Área Promotora

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin" 

  Esquema do Cenário: Validar a: <caso>
    E visualizo a tela "Área Promotora"
    Quando clico no cadastro da Área Promotora
    E depois clico na edição da promotora
    Então sistema edita a área promotora

  Exemplos:
    | caso                 |
    | Edição do cadastro |
