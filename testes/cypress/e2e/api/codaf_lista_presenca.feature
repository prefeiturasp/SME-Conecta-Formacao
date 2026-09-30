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

  Cenário: Criar lista de presença do Codaf
    Dado que possuo um token válido no endpoint CodafListaPresenca
    Quando envio uma requisição POST na lista presença do Codaf
    Então retorna o status 201 com os dados da lista de presença do Codaf

  Cenário: Não criar lista de presença do Codaf sem campos obrigatórios
    Dado que possuo um token válido no endpoint CodafListaPresenca
    Quando envio a requisição POST na lista do Codaf sem campos
    Então retorna o status 422 sem criar lista de presença do Codaf sem campos obrigatórios

  Cenário: Não criar lista de presença do Codaf sem autenticação
    Dado que não possuo um token válido
    Quando tento uma requisição POST na lista presença do Codaf
    Então retorna o status 401 sem criar lista de presença do Codaf

  Cenário: Alterar lista de presença do Codaf
    Dado que possuo um token válido no endpoint CodafListaPresenca
    Quando envio uma requisição PUT na lista presença do Codaf
    Então retorna o status 204 ao alterar a lista de presença do Codaf

  Cenário: Não alterar lista de presença do Codaf sem id
    Dado que possuo um token válido no endpoint CodafListaPresenca
    Quando envio uma requisição PUT na lista presença do Codaf sem id
    Então retorna o status 405 sem alterar a lista de presença do Codaf

  Cenário: Não altera lista de presença do Codaf sem autenticação
    Dado que não possuo um token válido
    Quando tento uma requisição PUT na lista presença do Codaf
    Então retorna o status 401 sem alterar a lista de presença do Codaf

  Cenário: Deletar lista de presença do Codaf
    Dado que possuo um token válido no endpoint CodafListaPresenca
    Quando envio uma requisição DELETE na lista presença do Codaf
    Então retorna o status 204 ao deletar a lista de presença do Codaf

  Cenário: Não deletar lista de presença do Codaf sem id
    Dado que possuo um token válido no endpoint CodafListaPresenca
    Quando envio uma requisição DELETE na lista presença do Codaf sem id
    Então retorna o status 405 sem deletar a lista de presença do Codaf

  Cenário: Não deletar lista de presença do Codaf sem autenticação
    Dado que não possuo um token válido
    Quando tento uma requisição DELETE na lista presença do Codaf
    Então retorna o status 401 sem deletar a lista de presença do Codaf