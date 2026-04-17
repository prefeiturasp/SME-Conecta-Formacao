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

  Cenário: Buscar dados de inscrição através do id
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição GET dados inscrição por proposta id
    Então retorna o status 200 com dados de inscrição através do id

  Cenário: Não buscar dados de inscrição através sem id
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição GET dados inscrição por proposta sem id
    Então não retorna o status 404 com dados de inscrição sem id

  Cenário: Não buscar dados de inscrição através do id sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET dados inscrição por proposta id
    Então retorna o status 401 sem dados de inscrição através do id

  Cenário: Buscar inscrição através da proposta id
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição GET inscricao por propostaId
    Então retorna o status 200 com inscrição através da proposta id

  Cenário: Não buscar inscrição através da proposta sem id
    Dado que possuo um token válido no endpoint Inscricao
    Quando requisição GET inscricao por proposta sem Id
    Então não retorna o status 404 inscrição através da proposta sem id

  Cenário: Não buscar inscrição através da proposta id sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET inscricao por propostaId
    Então retorna o status 401 sem inscrição através da proposta id

  Cenário: Buscar inscrição aberta através da proposta id
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição GET inscricao aberta por propostaId
    Então retorna o status 200 com inscrição abertas através da proposta id

  Cenário: Não buscar inscrição aberta através da proposta sem id
    Dado que possuo um token válido no endpoint Inscricao
    Quando requisição GET inscricao aberta por proposta sem Id
    Então não retorna o status 422 inscrição aberta através da proposta sem id

  Cenário: Não buscar inscrição aberta através da proposta id sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET inscricao aberta por propostaId
    Então retorna o status 401 sem inscrição aberta através da proposta id

  Cenário: Cadastrar inscrição com sucesso
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição POST para cadastrar inscrição
    Então retorna o status 200 com inscrição cadastrada com sucesso

  Cenário: Não cadastrar inscrição sem propostaTurmaId
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição POST para cadastrar inscrição sem propostaTurmaId
    Então retorna o status 422 ao tentar cadastrar inscrição sem propostaTurmaId

  Cenário: Não cadastrar inscrição sem autenticação
    Dado que não possuo um token válido
    Quando tento enviar uma requisição POST para cadastrar inscrição
    Então retorna o status 401 sem autenticação ao cadastrar inscrição

  Cenário: Cancelar inscrição com sucesso
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição PUT para cancelar inscrição
    Então retorna o status 200 com inscrição cancelada com sucesso

  Cenário: Não cancelar inscrição sem id
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição PUT para cancelar inscrição sem id
    Então retorna o status 415 ao tentar cancelar inscrição sem id

  Cenário: Não cancelar inscrição sem autenticação
    Dado que não possuo um token válido
    Quando tento enviar uma requisição PUT para cancelar inscrição
    Então retorna o status 401 sem autenticação ao cancelar inscrição

  Cenário: Cadastrar inscrição manual com sucesso
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição POST para cadastrar inscrição manual
    Então retorna o status 200 com inscrição manual cadastrada com sucesso

  Cenário: Não cadastrar inscrição manual sem propostaTurmaId
    Dado que possuo um token válido no endpoint Inscricao
    Quando envio uma requisição POST para cadastrar inscrição manual sem propostaTurmaId
    Então retorna o status 422 da inscrição manual não cadastrada sem propostaTurmaId

  Cenário: Não cadastrar inscrição manual sem autenticação
    Dado que não possuo um token válido
    Quando tento enviar uma requisição POST de inscrição manual
    Então retorna o status 401 de inscrição manual sem sucesso