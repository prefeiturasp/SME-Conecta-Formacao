# language: pt

Funcionalidade: Cadastro de lista de presença CODAF

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"

  Esquema do Cenário: Cadastrar lista de presença CODAF
    Quando acesso o menu Lista Presença Codaf
    E cadastro um Codaf homologado
    Então o sistema salva a nova lista de presença CODAF
