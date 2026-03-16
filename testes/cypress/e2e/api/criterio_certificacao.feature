# language: pt

Funcionalidade: API - Critério Certificação

  Cenário: Buscar os critérios de certificação
    Dado que possuo um token válido no endpoint CriterioCertificacao
    Quando envio uma requisição GET nas validações da certificação
    Então retorna o status 200 com critérios de certificação

  Cenário: Não buscar critérios de certificação sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET nas validações da certificação
    Então retorna o status 401 sem critérios de certificação