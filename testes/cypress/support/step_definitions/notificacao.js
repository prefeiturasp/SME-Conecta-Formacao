import { Given, When, Then, Before } from "@badeball/cypress-cucumber-preprocessor"

const Dado = Given
const Quando = When
const Então = Then

let token

Before(() => {
  cy.gerar_token().then((token_valido) => {
    token = token_valido
  })
})

Dado('que possuo um token no endpoint Notificacao', function () {
  expect(token, 'valido').to.exist
})

// Buscar notificações do usuário
Quando('envio uma requisição GET das notificações', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Notificacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna todas notificações com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)

    expect(response.body).to.have.property('items')
    expect(response.body.items).to.be.an('array').and.not.be.empty

    expect(response.body).to.have.property('totalPaginas')
    expect(response.body).to.have.property('totalRegistros')

    response.body.items.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item.id).to.be.a('number')

      expect(item).to.have.property('titulo')
      expect(item.titulo).to.be.a('string')

      expect(item).to.have.property('categoria')
      expect(item.categoria).to.be.a('number')

      expect(item).to.have.property('categoriaDescricao')
      expect(item.categoriaDescricao).to.be.a('string')

      expect(item).to.have.property('tipo')
      expect(item.tipo).to.be.a('number')

      expect(item).to.have.property('tipoDescricao')
      expect(item.tipoDescricao).to.be.a('string')

      expect(item).to.have.property('situacao')
      expect(item.situacao).to.be.a('number')

      expect(item).to.have.property('situacaoDescricao')
      expect(item.situacaoDescricao).to.be.a('string')
    })
  })
})

// Não buscar notificações do usuário sem autenticação
Quando('tento a requisição GET das notificações', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Notificacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem notificações', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar notificações por categoria
Quando('envio uma requisição GET na categoria das notificações', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Notificacao/categoria`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna todas notificações por categoria com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array').and.not.be.empty

    response.body.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item.id).to.be.a('number').and.greaterThan(0)
      expect(item).to.have.property('descricao').and.to.be.a('string').and.not.be.empty    
    })
  })
})

// Não buscar notificações por categoria sem autenticação
Quando('tento a requisição GET na categoria das notificações', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Notificacao/categoria`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem todas notificações por categoria', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar notificações por tipo
Quando('envio uma requisição GET no tipo das notificações', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Notificacao/tipo`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna todas notificações por tipo com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array').and.not.be.empty

    response.body.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item.id).to.be.a('number').and.greaterThan(0)
      expect(item).to.have.property('descricao').and.to.be.a('string').and.not.be.empty    
    })
  })
})

// Não buscar notificações por tipo sem autenticação
Quando('tento a requisição GET no tipo das notificações', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Notificacao/tipo`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem todas notificações por tipo', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar notificações por situação
Quando('envio uma requisição GET na situação das notificações', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Notificacao/situacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna todas notificações por situação com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array').and.not.be.empty

    response.body.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item.id).to.be.a('number').and.greaterThan(0)
      expect(item).to.have.property('descricao').and.to.be.a('string').and.not.be.empty    
    })
  })
})

// Não buscar notificações por situação sem autenticação
Quando('tento a requisição GET na situação das notificações', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Notificacao/situacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem todas notificações por situação', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar notificações não lida
Quando('envio uma requisição GET não lida das notificações', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Notificacao/nao-lida`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna todas notificações não lida com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.a('number')
    expect(response.body).to.be.greaterThan(0)
  })
})

// Não buscar notificações não lida sem autenticação
Quando('tento a requisição GET não lida das notificações', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Notificacao/nao-lida`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem todas notificações não lida', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar notificações por id
Quando('envio uma requisição GET no id notificações', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Notificacao/`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna as notificações por id com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)

    expect(response.body).to.have.property('items')
    expect(response.body.items).to.be.an('array').and.not.be.empty

    expect(response.body).to.have.property('totalPaginas')
    expect(response.body).to.have.property('totalRegistros')

    response.body.items.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item.id).to.be.a('number')

      expect(item).to.have.property('titulo')
      expect(item.titulo).to.be.a('string')

      expect(item).to.have.property('categoria')
      expect(item.categoria).to.be.a('number')

      expect(item).to.have.property('categoriaDescricao')
      expect(item.categoriaDescricao).to.be.a('string')

      expect(item).to.have.property('tipo')
      expect(item.tipo).to.be.a('number')

      expect(item).to.have.property('tipoDescricao')
      expect(item.tipoDescricao).to.be.a('string')

      expect(item).to.have.property('situacao')
      expect(item.situacao).to.be.a('number')

      expect(item).to.have.property('situacaoDescricao')
      expect(item.situacaoDescricao).to.be.a('string')
    })
  })
})

// Não buscar notificações por id inválido
Quando('envio uma requisição GET no id inexistente notificações', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Notificacao/0`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna as notificações por id inválido com status 400', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(400)
  })
})

// Não buscar notificações por id sem autenticação
Quando('tento a requisição GET no id notificações', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Notificacao/`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem todas notificações por id', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})