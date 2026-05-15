# language: pt

Funcionalidade: API - Usuário rede parceria

  Cenário: Buscar usuários rede parceria
    Dado que possuo um token válido no endpoint UsuarioRedeParceria
    Quando envio uma requisição GET buscar o usuário de parceria
    Então retorna o status 200 com usuários rede parceria

  Cenário: Não buscar usuários rede parceria sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET buscar o usuário de parceria
    Então retorna o status 401 sem usuários rede parceria
  
  Cenário: Buscar situação de usuários rede parceria
    Dado que possuo um token válido no endpoint UsuarioRedeParceria
    Quando envio uma requisição GET da situação do usuário parceria
    Então retorna o status 200 com situação de usuários rede parceria

  Cenário: Não buscar situação de usuários rede parceria sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET da situação de usuários parceria
    Então retorna o status 401 sem situação de usuários rede parceria

  Cenário: Buscar id de usuários rede parceria
    Dado que possuo um token válido no endpoint UsuarioRedeParceria
    Quando envio uma requisição GET id do usuário parceria
    Então retorna o status 200 com id de usuários rede parceria

  Cenário: Id de usuários rede parceria inválido
    Dado que possuo um token válido no endpoint UsuarioRedeParceria
    Quando envio uma requisição GET sem id do usuário parceria
    Então retorna o status 400 que id de usuários rede parceria é inválido

  Cenário: Não buscar id de usuários rede parceria sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET id de usuários parceria
    Então retorna o status 401 sem id de usuários rede parceria
  