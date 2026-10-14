# language: pt

Funcionalidade: Exclusão de lista de presença CODAF

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"

  Esquema do Cenário: Cancelar exclusão de cadastro 
    Quando acesso o menu Lista Presença Codaf
    E seleciono o Codaf homologado para exclusão
    Então cancelo sem excluir a lista de presença CODAF

  Esquema do Cenário: Não permitir a exclusão
    Quando acesso o menu Lista Presença Codaf
    E seleciono o Codaf homologado diferente de iniciado
    Então não permite excluir a lista de presença CODAF

  Esquema do Cenário: Excluir lista de presença CODAF
    Quando acesso o menu Lista Presença Codaf
    E clico no Codaf homologado para exclusão
    Então o sistema exclui a lista de presença CODAF  
