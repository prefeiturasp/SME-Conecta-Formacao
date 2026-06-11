# language: pt

Funcionalidade: API - Codaf arquivo

  Cenário: Buscar o modelo do termo do Codaf
    Dado que possuo um token válido no endpoint CodafArquivo
    Quando envio uma requisição GET no Codaf lista presença
    Então retorna o status 200 com modelo do termo do Codaf

  Cenário: Não buscar modelo do termo do Codaf sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET no Codaf lista presença
    Então retorna o status 401 sem modelo do termo do Codaf