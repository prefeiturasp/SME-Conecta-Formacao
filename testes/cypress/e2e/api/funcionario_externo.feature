# language: pt

Funcionalidade: API - Funcionário externo

  Cenário: Buscar funcionário externo
    Dado que possuo um token válido no endpoint FuncionarioExterno
    Quando envio uma requisição GET obter usuarios externos
    Então retorna o status 200 com funcionário externo

  Cenário: Não buscar funcionário externo sem documento
    Dado que possuo um token válido no endpoint FuncionarioExterno
    Quando envio uma requisição GET obter usuarios externos sem documento
    Então não busca funcionário externo

  Cenário: Não buscar funcionário externo inválido
    Dado que não possuo um token válido
    Quando tento a requisição GET obter usuarios externos inexistente
    Então retorna o status 204 sem funcionário externo inválido