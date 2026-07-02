# language: pt

Funcionalidade: API - Codaf Lista Presenca

  Cenário: Buscar dados de presença do Codaf
    Dado que possuo um token válido no endpoint CodafListaPresenca
    Quando envio uma requisição GET na lista presença do Codaf
    Então retorna o status 200 com dados de presença do Codaf

  Cenário: Não buscar dados de presença do Codaf sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET na lista presença do Codaf
    Então retorna o status 401 sem dados de presença do Codaf

  Cenário: Buscar dados por id da presença do Codaf
    Dado que possuo um token válido no endpoint CodafListaPresenca
    Quando envio uma requisição GET id lista presença do Codaf
    Então retorna o status 200 com dados por id de presença do Codaf

  Cenário: Não buscar dados por id inválido na presença do Codaf
    Dado que possuo um token válido no endpoint CodafListaPresenca
    Quando envio uma requisição GET id inválido lista presença do Codaf
    Então retorna o status 404 sem dados por id de presença do Codaf

  Cenário: Não buscar dados por id da presença do Codaf sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET id lista presença do Codaf
    Então retorna o status 401 sem dados por id de presença do Codaf

  Cenário: Imprimir lista presença do Codaf
    Dado que possuo um token válido no endpoint CodafListaPresenca
    Quando envio uma requisição POST de imprimir lista presença do Codaf
    Então retorna o status 200 imprimindo lista presença do Codaf

  Cenário: Não imprimir lista presença do Codaf sem id
    Dado que possuo um token válido no endpoint CodafListaPresenca
    Quando envio sem id na requisição POST de imprimir lista do Codaf
    Então retorna o status 404 sem dados sem imprimir lista presença do Codaf

  Cenário: Não imprimir lista presença do Codaf sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição POST de imprimir lista do Codaf
    Então retorna o status 401 sem imprimir lista presença do Codaf