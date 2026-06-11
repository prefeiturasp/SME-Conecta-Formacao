# language: pt

Funcionalidade: API - Palavra chave

  Cenário: Buscar cadastros de palavra chave
    Dado que possuo um token válido no endpoint PalavraChave
    Quando envio uma requisição GET buscar palavra chave
    Então retorna o status 200 com cadastros de palavra chave

  Cenário: Não buscar cadastros de palavra chave sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET buscar palavra chave
    Então retorna o status 401 sem palavra chave