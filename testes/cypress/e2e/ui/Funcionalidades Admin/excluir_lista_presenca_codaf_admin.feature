# language: pt

Funcionalidade: Exclusão de lista de presença CODAF

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"

  Esquema do Cenário: Excluir lista de presença CODAF
    Quando acesso o menu Lista Presença Codaf
    E clico no Codaf homologado para exclusão
    Então o sistema exclui a lista de presença CODAF
