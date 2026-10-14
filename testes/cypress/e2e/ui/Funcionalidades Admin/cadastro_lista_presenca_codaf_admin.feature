# language: pt

Funcionalidade: Cadastro de lista de presença CODAF

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"

  Esquema do Cenário: Cadastrar lista de presença CODAF
    Quando acesso o menu Lista Presença Codaf
    E cadastro um Codaf homologado
    Então o sistema salva a nova lista de presença CODAF

  Esquema do Cenário: Campos obrigatórios ao salvar registro
    Quando acesso o menu Lista Presença Codaf
    E tento cadastrar um Codaf homologado sem preenchimento
    Então o sistema informa campos obrigatórios na nova lista de presença CODAF

  Esquema do Cenário: Cancelar cadastro de novo registro
    Quando acesso o menu Lista Presença Codaf
    E cancelo o cadastro do Codaf homologado
    Então o sistema retorna a lista de presença CODAF

  Esquema do Cenário: Retornar para lista de presença CODAF
    Quando acesso o menu Lista Presença Codaf
    E clico em voltar no Codaf homologado
    Então o sistema retorna a listagem de presença CODAF
