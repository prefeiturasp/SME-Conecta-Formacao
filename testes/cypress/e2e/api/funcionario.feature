# language: pt

Funcionalidade: API - Funcionário

  Cenário: Buscar funcionários com usuários admin df
    Dado que possuo um token válido no endpoint Funcionario
    Quando envio uma requisição GET obter usuarios admin df
    Então retorna o status 200 funcionários com usuários admin df

  Cenário: Não buscar funcionários com usuários admin df sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET obter usuarios admin df
    Então retorna o status 401 sem funcionários com usuários admin df

  Cenário: Buscar funcionários com usuários parcerista
    Dado que possuo um token válido no endpoint Funcionario
    Quando envio uma requisição GET obter usuarios parcerista
    Então retorna o status 200 funcionários com usuários parcerista

  Cenário: Não buscar funcionários com usuários parcerista sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET obter usuarios parcerista
    Então retorna o status 401 sem funcionários com usuários parcerista