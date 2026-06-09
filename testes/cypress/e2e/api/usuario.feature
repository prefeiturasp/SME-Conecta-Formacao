# language: pt

Funcionalidade: API - Usuário

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

  Cenário: Solicitar recuperação de senha
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição POST com login do usuário
    Então retorna o status 200 solicitando recuperação de senha

  Cenário: Não solicitar recuperação de senha sem usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição POST sem login do usuário
    Então retorna o status 405 sem solicitar recuperação de senha

  Cenário: Não solicitar recuperação de senha com usuário inválido
    Dado que não possuo um token válido
    Quando envio uma requisição POST com login de usuário inválido
    Então retorna o status 400 sem solicitar recuperação de senha

  Cenário: Validar token de recuperação de senha
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição GET com token da senha
    Então retorna o status 200 com token de recuperação de senha

  Cenário: Não validar recuperação de senha sem token
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição GET sem token da senha
    Então retorna o status 422 sem token de recuperação de senha

  Cenário: Não validar token inválido na recuperação de senha
    Dado que não possuo um token válido
    Quando envio uma requisição GET com token de recuperação
    Então retorna o status 401 sem validar token de recuperação de senha

  Cenário: Recuperar senha do usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição PUT de recuperar senha
    Então retorna o status 200 recuperando nova senha

  Cenário: Não recuperar senha sem token
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição PUT de recuperar senha sem token
    Então retorna o status 400 sem recuperação da senha

  Cenário: Não recuperar sem inserir nova senha
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição PUT de recuperar sem a senha
    Então retorna o status 400 sem recuperar a senha

  Cenário: Reenviar e-mail de recuperação de senha ao usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando envio uma requisição GET de reenvio da senha
    Então retorna o status 200 reenviando o e-mail de senha ao usuário

  Cenário: Não reenviar e-mail de recuperação de senha sem usuário
    Dado que possuo um token válido no endpoint Usuario
    Quando tento a requisição GET de reenvio da senha
    Então retorna o status 405 sem reenviar e-mail de senha
