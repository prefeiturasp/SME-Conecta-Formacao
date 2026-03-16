import { Given, When, Then, Before } from "@badeball/cypress-cucumber-preprocessor"

let token

Before(() => {
  cy.gerar_token().then((token_valido) => {
    token = token_valido
  })
})

Given('que possuo um token válido no endpoint Inscricao', function () {
  expect(token, 'valido').to.exist
})

// Buscar dados de inscrição
When('envio uma requisição GET dos dados de inscrição', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/dados-inscricao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com dados de inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('object')
    expect(response.body).to.have.property('usuarioNome')
    expect(response.body).to.have.property('usuarioRf')
    expect(response.body).to.have.property('usuarioCpf')
    expect(response.body).to.have.property('usuarioEmail')
    expect(response.body).to.have.property('usuarioCargos')
  })
})

// Não buscar dados de inscrição sem autenticação
When('tento a requisição GET dos dados de inscrição', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/dados-inscricao`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 401 sem dados de inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar inscrição
When('envio uma requisição GET na inscrição', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com a inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('object')
    expect(response.body).to.have.property('items')
    expect(response.body.items).to.be.an('array')

    response.body.items.forEach((item) => {
      expect(item).to.have.property('codigoFormacao')
      expect(item).to.have.property('nomeFormacao')
      expect(item).to.have.property('nomeTurma')
      expect(item).to.have.property('datas')
      expect(item).to.have.property('cargoFuncaoCodigo')
    })
  })
})

// Não buscar inscrição sem autenticação
When('tento a requisição GET na inscrição', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 401 sem a inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar próximas inscrições
When('envio uma requisição GET em próximas inscrições', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/proximas`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com próximas inscrições', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('object')
    expect(response.body).to.have.property('items')
    expect(response.body.items).to.be.an('array')

    response.body.items.forEach((item) => {
      expect(item).to.have.property('codigoFormacao')
      expect(item).to.have.property('nomeFormacao')
      expect(item).to.have.property('nomeTurma')
      expect(item).to.have.property('datas')
      expect(item).to.have.property('cargoFuncaoCodigo')
    })
  })
})

// Não buscar próximas inscrições sem autenticação
When('tento a requisição GET em próximas inscrições', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/proximas`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 401 sem próximas inscrições', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar inscrição finalizada
When('envio uma requisição GET em inscrição encerradas', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/finalizadas`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com inscrição finalizada', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('object')
    expect(response.body).to.have.property('items')
    expect(response.body.items).to.be.an('array')

    response.body.items.forEach((item) => {
      expect(item).to.have.property('codigoFormacao')
      expect(item).to.have.property('nomeFormacao')
      expect(item).to.have.property('nomeTurma')
      expect(item).to.have.property('datas')
      expect(item).to.have.property('cargoFuncaoCodigo')
    })
  })
})

// Não buscar inscrição finalizada sem autenticação
When('tento a requisição GET em inscrição encerradas', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/finalizadas`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 401 sem inscrição finalizada', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar formação de turmas
When('envio uma requisição GET em turma formadas', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/formacao-turmas`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com formação de turmas', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(404)    
  })
})

// Não buscar formação de turmas sem autenticação
When('tento a requisição GET em turma formadas', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/formacao-turmas`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 401 sem formação de turmas', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar tipos de inscrição
When('envio uma requisição GET em inscrição tipos', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/tipos`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com tipos de inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array')    
    expect(response.body[0]).to.have.property('id')
    expect(response.body[0]).to.have.property('descricao')
  })
})

// Não buscar tipos de inscrição sem autenticação
When('tento a requisição GET em inscrição tipos', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/tipos`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 401 sem tipos de inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar inscrição de cursista
When('envio uma requisição GET no cursista em inscrição', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/cursista?cpf=25828579800`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com inscrição de cursista', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.not.be.null
    expect(response.body).to.be.an('object')
    expect(response.body).to.have.property('cpf')
    expect(response.body).to.have.property('nome')
  })
})

// Não buscar inscrição de cursista sem autenticação
When('tento a requisição GET no cursista em inscrição', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/cursista?cpf=25828579800`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 401 sem inscrição de cursista', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})