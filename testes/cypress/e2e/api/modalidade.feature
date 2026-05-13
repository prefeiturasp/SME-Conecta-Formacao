# language: pt

Funcionalidade: API - Modalidade

  Cenário: Buscar cadastros de modalidades
    Dado que possuo um token válido no endpoint Modalidade
    Quando envio uma requisição GET buscar modalidades
    Então retorna o status 200 com cadastros de modalidades

  Cenário: Não buscar cadastros de modalidades sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET buscar modalidades
    Então retorna o status 401 sem cadastros de modalidades