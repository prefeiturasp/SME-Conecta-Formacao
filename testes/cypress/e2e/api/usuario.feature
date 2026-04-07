# language: pt

Funcionalidade: API - Usuario

  Cenário: Buscar cadastro do usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição GET buscar o usuário
    Então retorna o status 200 com cadastro do usuário

  Cenário: Não buscar cadastro do usuário inválido
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição GET buscar sem usuário válido
    Então retorna o status 405 sem cadastro do usuário

  Cenário: Não buscar cadastro do usuário sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET buscar o usuário
    Então retorna o status 401 sem cadastro do usuário

  Cenário: Validar e-mail do usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição GET com o token nos dados
    Então retorna o status 200 validando e-mail do usuário

  Cenário: Não validar e-mail do usuário com token inválido
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição GET sem token válido nos dados
    Então retorna o status 405 sem validar e-mail do usuário

  Cenário: Não validar e-mail do usuário sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET com o token nos dados
    Então retorna o status 401 sem validar e-mail do usuário

  Cenário: Buscar tipo de e-mail do usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição GET tipo de e-mail
    Então retorna o status 200 com tipo de e-mail do usuário

  Cenário: Alterar senha do usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição PUT com usuário da senha
    Então retorna o status 200 alterando a senha do usuário

  Cenário: Não alterar senha sem usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição PUT sem usuário da senha
    Então retorna o status 405 sem alterar senha do usuário

  Cenário: Não alterar senha sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição PUT com usuário da senha
    Então retorna o status 401 sem alterar senha do usuário

  Cenário: Alterar e-mail do usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição PUT com usuário do e-mail
    Então retorna o status 200 alterando o e-mail do usuário

  Cenário: Não alterar e-mail sem o dado
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição PUT do usuário sem e-mail
    Então retorna o status 422 sem alterar o e-mail de usuário

  Cenário: Não alterar e-mail sem usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição PUT sem usuário do e-mail
    Então retorna o status 422 sem alterar o e-mail do usuário

  Cenário: Não alterar e-mail sem senha
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição PUT sem senha do e-mail
    Então retorna o status 422 sem alterar e-mail para usuário

  Cenário: Não alterar e-mail sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição PUT com usuário do e-mail
    Então retorna o status 401 sem alterar o e-mail do usuário

  Cenário: Alterar e-mail com usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição PUT com usuário para e-mail
    Então retorna o status 200 alterando e-mail do usuário

  Cenário: Não alterar e-mail sem o dado no usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição PUT Usuario sem e-mail
    Então retorna o status 422 sem alterar o e-mail de Usuario

  Cenário: Não alterar e-mail sem usuário na requisição
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição PUT sem usuário do campo email
    Então retorna o status 422 sem alterar email do usuário

  Cenário: Não alterar email sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição PUT com usuário para e-mail
    Então retorna o status 401 sem alterar email do usuário

  Cenário: Alterar nome do usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição PUT com nome do usuário
    Então retorna o status 200 alterando nome do usuário

  Cenário: Não alterar nome sem o dado no usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição PUT Usuario sem nome
    Então retorna o status 422 sem alterar nome de Usuario

  Cenário: Não alterar nome sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição PUT com nome do usuário
    Então retorna o status 401 sem alterar nome do usuário