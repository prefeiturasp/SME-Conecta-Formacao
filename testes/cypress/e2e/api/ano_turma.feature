# language: pt

Funcionalidade: API - Ano turma com modalidade

  Cenário: Buscar dados do ano turma da modalidade 1
    Dado que possuo um token de acesso
    Quando envio uma requisição GET com ano letivo da modalidade 1
    Então retorna o status 200 com dados do ano turma da modalidade 1

  Cenário: Buscar dados do ano turma da modalidade 3
    Dado que possuo um token de acesso
    Quando envio uma requisição GET com ano letivo da modalidade 3
    Então retorna o status 200 com dados do ano turma da modalidade 3

  Cenário: Buscar dados do ano turma da modalidade 4
    Dado que possuo um token de acesso
    Quando envio uma requisição GET com ano letivo da modalidade 4
    Então retorna o status 200 com dados do ano turma da modalidade 4

  Cenário: Buscar dados do ano turma da modalidade 5
    Dado que possuo um token de acesso
    Quando envio uma requisição GET com ano letivo da modalidade 5
    Então retorna o status 200 com dados do ano turma da modalidade 5

  Cenário: Buscar dados do ano turma da modalidade 6
    Dado que possuo um token de acesso
    Quando envio uma requisição GET com ano letivo da modalidade 6
    Então retorna o status 200 com dados do ano turma da modalidade 6

  Cenário: Buscar dados do ano turma da modalidade 7
    Dado que possuo um token de acesso
    Quando envio uma requisição GET com ano letivo da modalidade 7
    Então retorna o status 200 com dados do ano turma da modalidade 7

  Cenário: Buscar dados do ano turma da modalidade 8
    Dado que possuo um token de acesso
    Quando envio uma requisição GET com ano letivo da modalidade 8
    Então retorna o status 200 com dados do ano turma da modalidade 8

  Cenário: Buscar dados do ano turma da modalidade 9
    Dado que possuo um token de acesso
    Quando envio uma requisição GET com ano letivo da modalidade 9
    Então retorna o status 200 com dados do ano turma da modalidade 9

  Cenário: Buscar dados do ano turma da modalidade 10
    Dado que possuo um token de acesso
    Quando envio uma requisição GET com ano letivo da modalidade 10
    Então retorna o status 200 com dados do ano turma da modalidade 10

  Cenário: Não retorna dados de ano turma com modalidade inválida
    Dado que possuo um token de acesso
    Quando envio uma requisição GET com ano letivo da modalidade inválida
    Então retorna o status 422 sem dados do ano turma da modalidade

  Cenário: Não retorna dados de ano turma sem modalidade
    Dado que possuo um token de acesso
    Quando envio uma requisição GET com ano letivo sem modalidade
    Então retorna o status 422 sem dados do ano turma de modalidade

  Cenário: Não retorna dados de ano turma sem ano letivo
    Dado que possuo um token de acesso
    Quando envio uma requisição GET com ano letivo sem ano letivo
    Então retorna o status 500 sem dados do ano turma modalidade

  Cenário: Não buscar dados do ano turma da modalidade sem autenticação
    Dado que não possuo um token de acesso
    Quando tento a requisição GET com ano letivo da modalidade
    Então retorna o status 401 sem dados do ano turma da modalidade