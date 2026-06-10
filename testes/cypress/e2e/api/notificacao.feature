# language: pt

Funcionalidade: API - Notificacão

  Cenário: Buscar notificações do usuário
    Dado que possuo um token no endpoint Notificacao
    Quando envio uma requisição GET das notificações
    Então retorna todas notificações com status 200

  Cenário: Não buscar notificações do usuário sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET das notificações
    Então retorna o status 401 sem notificações

  Cenário: Buscar notificações por categoria
    Dado que possuo um token no endpoint Notificacao
    Quando envio uma requisição GET na categoria das notificações
    Então retorna todas notificações por categoria com status 200

  Cenário: Não buscar notificações por categoria sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET na categoria das notificações
    Então retorna o status 401 sem todas notificações por categoria

  Cenário: Buscar notificações por tipo
    Dado que possuo um token no endpoint Notificacao
    Quando envio uma requisição GET no tipo das notificações
    Então retorna todas notificações por tipo com status 200

  Cenário: Não buscar notificações por tipo sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET no tipo das notificações
    Então retorna o status 401 sem todas notificações por tipo

  Cenário: Buscar notificações por situação
    Dado que possuo um token no endpoint Notificacao
    Quando envio uma requisição GET na situação das notificações
    Então retorna todas notificações por situação com status 200

  Cenário: Não buscar notificações por situação sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET na situação das notificações
    Então retorna o status 401 sem todas notificações por situação

  Cenário: Buscar notificações não lida
    Dado que possuo um token no endpoint Notificacao
    Quando envio uma requisição GET não lida das notificações
    Então retorna todas notificações não lida com status 200

  Cenário: Não buscar notificações não lida sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET não lida das notificações
    Então retorna o status 401 sem todas notificações não lida

  Cenário: Buscar notificações por id
    Dado que possuo um token no endpoint Notificacao
    Quando envio uma requisição GET no id notificações
    Então retorna as notificações por id com status 200

  Cenário: Não buscar notificações por id inválido
    Dado que possuo um token no endpoint Notificacao
    Quando envio uma requisição GET no id inexistente notificações
    Então retorna as notificações por id inválido com status 400

  Cenário: Não buscar notificações por id sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET no id notificações
    Então retorna o status 401 sem todas notificações por id