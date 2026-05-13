# language: pt

Funcionalidade: API - Autenticação

  Cenário: Realiza a autenticação com sucesso
    Dado que acesso o endpoint de autenticação
    Quando envio os dados de acesso
    Então retorna status 200 com o token válido

  Cenário: Login deve ser obrigatório
    Dado que acesso o endpoint de autenticação
    Quando envio os dados sem o login
    Então retorna status 422 que acesso foi inválido

  Cenário: Senha deve ser obrigatória
    Dado que acesso o endpoint de autenticação
    Quando envio os dados sem a senha
    Então retorna status 422 que é necessário ser informada

  Cenário: Não autenticar com senha inválida
    Dado que acesso o endpoint de autenticação
    Quando envio os dados com senha inválida
    Então retorna status 401 retorna a mensagem que está incorreta

  Cenário: Deve revalidar o token do usuário
    Dado que possuo um token de acesso válido
    Quando envio uma requisição POST para revalidar o token
    Então retorna a expiração com status 200 

  Cenário: Não revalidar token inválido
    Dado que possuo um token de acesso válido
    Quando tento a requisição POST para revalidar o token
    Então não revalida retornando o status 401 de inválido

  Cenário: Selecionar perfil válido para o usuário
    Dado que login gerou um token de acesso válido
    Quando envio uma requisição PUT para o endpoint de autenticação do perfil
    Então retorna o id com status 200

  Cenário: Não permitir selecionar perfil inválido
    Dado que login gerou um token de acesso válido
    Quando tento a requisição PUT para o endpoint com perfil inválido
    Então retorna o status 422 que não existente

  Cenário: Não permitir perfil vazio
    Dado que login gerou um token de acesso válido
    Quando tento a requisição PUT para o endpoint sem perfil inválido
    Então retorna o status 404 que não foi selecionado

  Cenário: Não selecionar perfil sem autenticação
    Dado que não login não gerou um token de acesso válido
    Quando tento a requisição PUT para o endpoint de autenticação do perfil
    Então retorna o status 401