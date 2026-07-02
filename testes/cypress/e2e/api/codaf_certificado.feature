# language: pt

Funcionalidade: API - Codaf Certificado

  Cenário: Retornar todos meus certificados
    Dado que possuo um token válido do CodafCertificado
    Quando envio uma requisição GET no endpoint meus CodafCertificado
    Então retorna o status 200 com meus certificados

  Cenário: Retornar meus certificados como cursista
    Dado que possuo um token válido do CodafCertificado
    Quando envio uma requisição GET no endpoint meus CodafCertificado cursista
    Então retorna o status 200 com meus certificados de cursista

  Cenário: Não retornar meus certificados sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET no endpoint meus CodafCertificado
    Então retorna o status 401 sem meus certificados

  Cenário: Retornar Codaf certificados
    Dado que possuo um token válido do CodafCertificado
    Quando envio uma requisição GET no endpoint CodafCertificado
    Então retorna o status 200 com Codaf certificados

  Cenário: Retornar Codaf certificados de cursista
    Dado que possuo um token válido do CodafCertificado
    Quando envio uma requisição GET no endpoint CodafCertificado cursista
    Então retorna o status 200 com Codaf certificados de cursista

  Cenário: Não retornar Codaf certificados sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET no endpoint CodafCertificado
    Então retorna o status 401 sem Codaf certificados

  Cenário: Retornar download Codaf certificados
    Dado que possuo um token válido do CodafCertificado
    Quando envio uma requisição GET no endpoint CodafCertificado download
    Então retorna o status 200 com download Codaf certificados

  Cenário: Id Codaf obrigatório para download certificados
    Dado que possuo um token válido do CodafCertificado
    Quando envio uma requisição GET no endpoint sem id CodafCertificado download
    Então retorna o status 404 sem download Codaf certificados

  Cenário: Não retornar download Codaf certificados sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET no endpoint CodafCertificado download
    Então retorna o status 401 sem download Codaf certificados

  Cenário: Emitir download por lista presença Codaf certificados
    Dado que possuo um token válido do CodafCertificado
    Quando envio uma requisição POST no endpoint CodafCertificado download lista
    Então retorna o status 204 com emitindo download lista presença Codaf certificados

  Cenário: Não emitir download sem a lista presença Codaf certificados
    Dado que possuo um token válido do CodafCertificado
    Quando envio uma requisição POST no endpoint CodafCertificado download sem lista
    Então retorna o status 404 sem download lista presença Codaf certificados

  Cenário: Não emitir download por lista presença Codaf certificados sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição POST no endpoint CodafCertificado download lista
    Então retorna o status 401 sem emitir download lista presença Codaf certificados

  Cenário: Emitir download lote Codaf certificados
    Dado que possuo um token válido do CodafCertificado
    Quando envio uma requisição POST no endpoint CodafCertificado download lote
    Então retorna o status 200 com emitindo download lote Codaf certificados

  Cenário: Não emitir download lote Codaf certificados sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição POST no endpoint CodafCertificado download lote
    Então retorna o status 401 sem emitir download lote Codaf certificados