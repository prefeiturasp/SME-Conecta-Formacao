# language: pt

Funcionalidade: API - Proposta

  Cenário: Criar uma nova proposta
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição POST em Proposta
    Então retorna o status 200 criando uma nova proposta

  Cenário: Não criar proposta sem dados obrigatórios
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição POST em Proposta sem dados
    Então retorna o status 400 sem criar proposta

  Cenário: Não criar proposta sem autenticação
    Dado que possuo um token válido no endpoint Proposta
    Quando tento uma requisição POST em Proposta
    Então retorna o status 401 sem criar proposta

  Cenário: Atualizar proposta
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição PUT em Proposta
    Então retorna o status 200 atualizando a proposta

  Cenário: Não atualizar proposta sem o id
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição PUT em Proposta sem id
    Então retorna o status 405 sem atualizar a proposta

  Cenário: Não atualizar proposta sem autenticação
    Dado que possuo um token válido no endpoint Proposta
    Quando tento uma requisição PUT em Proposta
    Então retorna o status 401 sem atualizar a proposta 

  Cenário: Buscar informações do cadastrante
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição GET na informações cadastrante
    Então retorna o status 200 com informações do cadastrante

  Cenário: Não buscar informações do cadastrante sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET na informações cadastrante
    Então retorna o status 401 sem informações do cadastrante

  Cenário: Buscar roteiro
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição GET roteiro
    Então retorna o status 200 com roteiro

  Cenário: Não buscar roteiro sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET roteiro
    Então retorna o status 401 sem roteiro

  Cenário: Buscar critério validação de inscrição 
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição GET criterio validacao inscricao 
    Então retorna o status 200 com critério validação de inscrição 

  Cenário: Não buscar critério validação de inscrição sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET criterio validacao inscricao
    Então retorna o status 401 sem critério validação de inscrição

  Cenário: Buscar tipo de formação 
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição GET tipo formacao
    Então retorna o status 200 com tipo de formação

  Cenário: Não buscar tipo de formação sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET tipo formacao
    Então retorna o status 401 sem tipo de formação

  Cenário: Buscar tipo de inscrição 
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição GET tipo inscricao
    Então retorna o status 200 com tipo de inscrição

  Cenário: Não buscar tipo de inscrição sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET tipo inscricao
    Então retorna o status 401 sem tipo de inscrição

  Cenário: Buscar situação
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição GET situacao
    Então retorna o status 200 com situação

  Cenário: Não buscar situação sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET situacao
    Então retorna o status 401 sem situação

  Cenário: Buscar tipo encontro
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição GET tipo encontro
    Então retorna o status 200 com tipo encontro

  Cenário: Não buscar tipo encontro sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET tipo encontro
    Então retorna o status 401 sem tipo encontro

  Cenário: Buscar formação homologada
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição GET formacao homologada
    Então retorna o status 200 com formação homologada

  Cenário: Não buscar formação homologada sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET formacao homologada
    Então retorna o status 401 sem formação homologada

  Cenário: Buscar proposta
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição GET proposta
    Então retorna o status 200 com proposta

  Cenário: Não buscar proposta sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET proposta
    Então retorna o status 401 sem proposta

  Cenário: Buscar dashboard
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição GET dashboard
    Então retorna o status 200 com dashboard

  Cenário: Não buscar dashboard sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET dashboard
    Então retorna o status 401 sem dashboard

  Cenário: Buscar horas totais proposta
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição GET horas totais proposta
    Então retorna o status 200 com horas totais proposta

  Cenário: Não buscar horas totais proposta sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET horas totais proposta
    Então retorna o status 401 sem horas totais proposta

  Cenário: Buscar autocompletar formação
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição GET autocompletar formacao
    Então retorna o status 200 com autocompletar formação

  Cenário: Não buscar autocompletar formação sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET autocompletar formacao
    Então retorna o status 401 sem autocompletar formação

  Cenário: Excluir proposta
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição DELETE em Proposta
    Então retorna o status 200 com exclusão da proposta

  Cenário: Não excluir proposta sem o id
    Dado que possuo um token válido no endpoint Proposta
    Quando envio uma requisição DELETE em Proposta sem id
    Então retorna o status 405 sem excluir proposta

  Cenário: Não excluir proposta sem autenticação
    Dado que não possuo um token válido
    Quando tento uma requisição DELETE em Proposta
    Então retorna o status 401 sem excluir a proposta