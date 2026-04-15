# language: pt

Funcionalidade: API - Grupo

  Cenário: Buscar cadastros de grupo
    Dado que possuo um token válido no endpoint Grupo
    Quando envio uma requisição GET buscar os grupos
    Então retorna o status 200 com cadastros de grupo

  Cenário: Não buscar cadastros de grupo sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET buscar os grupos
    Então retorna o status 401 sem cadastros de grupo