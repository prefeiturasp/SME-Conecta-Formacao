import { Given, When, Then, Before } from "@badeball/cypress-cucumber-preprocessor"

// Realiza a autenticação com sucesso
Given('que acesso o endpoint de autenticação', function () {  
})

When('envio os dados de acesso', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/autenticacao`,
    headers: {
      accept: 'text/plain',
      'content-type': 'application/json',
    },
    body: {
      login: `${Cypress.env('LOGIN_ADM_GERAL')}`,
      senha: `${Cypress.env('SENHA')}`
    },
    failOnStatusCode: false
  }).as('response')
})

Then('retorna status 200 com o token válido', function () {
  cy.get('@response').then((response) => {
    expect([200]).to.include(response.status)
    expect(response.body).to.have.property('usuarioNome')
    expect(response.body).to.have.property('usuarioLogin')
    expect(response.body).to.have.property('token')
    expect(response.body).to.have.property('dataHoraExpiracao')
    expect(response.body).to.have.property('email')
    expect(response.body).to.have.property('autenticado')
    expect(response.body).to.have.property('perfilUsuario')
  })
})

// Login deve ser obrigatório
When('envio os dados sem o login', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/autenticacao`,
    headers: {
      accept: 'text/plain',
      'content-type': 'application/json',
    },
    body: {
      login:" ",
      senha: `${Cypress.env('SENHA')}`
    },
    failOnStatusCode: false
  }).as('response')
})

Then('retorna status 422 que acesso foi inválido', function () {
  cy.get('@response').then((response) => {
    expect([422]).to.include(response.status)
    expect(response.body).to.have.property('mensagens')
    expect(response.body).to.have.property('existemErros')
    expect(response.body.mensagens).to.include("É necessário informar o login.")
    expect(response.body.mensagens).to.include("O login deve conter no mínimo 5 caracteres.")
  })
})

// Senha deve ser obrigatória
When('envio os dados sem a senha', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/autenticacao`,
    headers: {
      accept: 'text/plain',
      'content-type': 'application/json',
    },
    body: {
      login: `${Cypress.env('LOGIN_ADM_GERAL')}`,
      senha:""
    },
    failOnStatusCode: false
  }).as('response')
})

Then('retorna status 422 que é necessário ser informada', function () {
  cy.get('@response').then((response) => {
    expect([422]).to.include(response.status)
    expect(response.body).to.have.property('mensagens')
    expect(response.body).to.have.property('existemErros')
    expect(response.body.mensagens).to.include("É necessário informar a senha.")
    expect(response.body.mensagens).to.include("A senha deve conter no mínimo 4 caracteres.")
    expect(response.body.existemErros).to.be.true
  })
})

// Não autenticar com senha inválida
When('envio os dados com senha inválida', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/autenticacao`,
    headers: {
      accept: 'text/plain',
      'content-type': 'application/json',
    },
    body: {
      login: `${Cypress.env('LOGIN_ADM_GERAL')}`,
      senha: `${Cypress.env('123')}`
    },
    failOnStatusCode: false
  }).as('response')
})

Then('retorna status 401 retorna a mensagem que está incorreta', function () {
  cy.get('@response').then((response) => {
    expect([401]).to.include(response.status)
    expect(response.body).to.have.property('mensagens')
    expect(response.body).to.have.property('existemErros')
    expect(response.body.mensagens).to.include("Usuário ou senha inválidos")
    expect(response.body.existemErros).to.be.true
  })
})

// Deve revalidar o token do usuário
Given('que possuo um token de acesso válido', function () {
})

When('envio uma requisição POST para revalidar o token', function () {
  cy.gerar_token().then((token) => {
    cy.request({
      method: 'POST',
      url: Cypress.config('baseUrl') + '/api/v1/autenticacao/revalidar',
      headers: {
        accept: 'text/plain',
        'content-Type': 'application/json'
      },
      body: {
        token: token 
      },
      failOnStatusCode: false
    }).as('response')
  })
})

Then('retorna a expiração com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.have.property('dataHoraExpiracao')
    expect(response.body).to.have.property('token')
  })
})

// Não revalidar token inválido
When('tento a requisição POST para revalidar o token', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/autenticacao/revalidar`,
    headers: {
      accept: 'text/plain',
      'content-Type': 'application/json'
    },
     body: {
      token: 'token_invalido'
    },
    failOnStatusCode: false
  }).as('response')
})

Then('não revalida retornando o status 401 de inválido', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
    expect(response.body).to.have.property('existemErros', true)
    expect(response.body).to.have.property('mensagens').that.is.an('array').and.not.empty
    expect(response.body.mensagens[0]).to.eq("Token inválido") 
  })
})

let token

Before(() => {
  cy.gerar_token().then((token_valido) => {
    token = token_valido
  })
})

Given('que login gerou um token de acesso válido', function () {
  expect(token, 'valido').to.exist
})

// Selecionar perfil válido para o usuário
When('envio uma requisição PUT para o endpoint de autenticação do perfil', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/autenticacao/perfis/7eda4540-a16c-4fe5-8322-9f75b3414e27`,
    headers: {
      accept: 'application/json',
      Authorization: `Bearer ${token}`
    },
    failOnStatusCode: false
  }).as('response')
})

Then('retorna o id com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.have.property('usuarioNome').and.not.be.empty
    expect(response.body).to.have.property('usuarioLogin').and.not.be.empty
    expect(response.body).to.have.property('dataHoraExpiracao').and.not.be.empty
    expect(response.body).to.have.property('token').and.not.be.empty
    expect(response.body).to.have.property('email').and.not.be.empty
    expect(response.body).to.have.property('autenticado').and.to.be.true
    expect(response.body).to.have.property('perfilUsuario').and.not.be.empty
  })
})

// Não permitir selecionar perfil inválido
When('tento a requisição PUT para o endpoint com perfil inválido', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/autenticacao/perfis/d3766fb4-d753-4398-bfb0-c357724bb0a`,
    headers: {
      accept: 'application/json',
      Authorization: `Bearer ${token}`
    },
    failOnStatusCode: false
  }).as('response')
})

Then('retorna o status 422 que não existente', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422)
    expect(response.body).to.have.property('existemErros', true)
  })
})

// Não permitir perfil vazio
When('tento a requisição PUT para o endpoint sem perfil inválido', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/autenticacao/perfis/`,
    headers: {
      accept: 'application/json',
      Authorization: `Bearer ${token}`
    },
    failOnStatusCode: false
  }).as('response')
})

Then('retorna o status 404 que não foi selecionado', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(404)
  })
})

// Não selecionar perfil sem autenticação
Given('que não login não gerou um token de acesso válido', () => {
  token = 'token_invalido'
})

When('tento a requisição PUT para o endpoint de autenticação do perfil', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/autenticacao/perfis/7eda4540-a16c-4fe5-8322-9f75b3414e27`,
    headers: {
     accept: 'application/json',
     Authorization: 'Bearer token_invalido'
    },
    failOnStatusCode: false
  }).as('response')
})

Then('retorna o status 401', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})