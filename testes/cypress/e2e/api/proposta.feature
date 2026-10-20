# language: pt

Funcionalidade: API - Proposta

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