# language: pt

Funcionalidade: API - Dre

  Cenário: Buscar cadastros de Dre
    Dado que possuo um token válido
    Quando envio uma requisição GET no endpoint Dre
    Então retorna o status 200 com todos cadastros de Dre

  Cenário: Buscar cadastros de Dre exibindo todas
    Dado que possuo um token válido
    Quando envio uma requisição GET no endpoint Dre como true
    Então retorna o status 200 com dados de Dre exibindo todas

  Cenário: Buscar cadastros de Dre não exibindo todas
    Dado que possuo um token válido
    Quando envio uma requisição GET no endpoint Dre como false
    Então retorna o status 200 com dados de Dre não exibindo todas

  Cenário: Não buscar cadastros de Dre sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET no endpoint Dre
    Então retorna o status 401 sem cadastros de Dre