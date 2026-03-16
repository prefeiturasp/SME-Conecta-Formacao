# language: pt

Funcionalidade: API - Inscrição

  Cenário: Buscar dados de inscrição
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição GET dos dados de inscrição
    Então retorna o status 200 com dados de inscrição

  Cenário: Não buscar dados de inscrição sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET dos dados de inscrição
    Então retorna o status 401 sem dados de inscrição

  Cenário: Buscar inscrição
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição GET na inscrição
    Então retorna o status 200 com a inscrição

  Cenário: Não buscar inscrição sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET na inscrição
    Então retorna o status 401 sem a inscrição

  Cenário: Buscar próximas inscrições
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição GET em próximas inscrições
    Então retorna o status 200 com próximas inscrições

  Cenário: Não buscar próximas inscrições sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET em próximas inscrições
    Então retorna o status 401 sem próximas inscrições

  Cenário: Buscar inscrição finalizada
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição GET em inscrição encerradas
    Então retorna o status 200 com inscrição finalizada

  Cenário: Não buscar inscrição finalizada sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET em inscrição encerradas
    Então retorna o status 401 sem inscrição finalizada

  Cenário: Buscar formação de turmas
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição GET em turma formadas
    Então retorna o status 200 com formação de turmas

  Cenário: Não buscar formação de turmas sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET em turma formadas
    Então retorna o status 401 sem formação de turmas

  Cenário: Buscar tipos de inscrição
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição GET em inscrição tipos
    Então retorna o status 200 com tipos de inscrição

  Cenário: Não buscar tipos de inscrição sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET em inscrição tipos
    Então retorna o status 401 sem tipos de inscrição

  Cenário: Buscar inscrição de cursista
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição GET no cursista em inscrição
    Então retorna o status 200 com inscrição de cursista

  Cenário: Não buscar inscrição de cursista sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET no cursista em inscrição
    Então retorna o status 401 sem inscrição de cursista