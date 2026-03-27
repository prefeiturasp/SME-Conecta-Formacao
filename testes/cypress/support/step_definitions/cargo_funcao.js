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

Dado('que possuo um token no endpoint CargoFuncao', function () {
  expect(token, 'valido').to.exist
})

// Buscar cargo função
Quando('envio uma requisição GET em cargos', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CargoFuncao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna todos cargo função com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array')

    response.body.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item.id).to.be.a('number').and.greaterThan(0)
      expect(item).to.have.property('nome').and.to.be.a('string')
      expect(item).to.have.property('tipo')
      expect(item).to.have.property('outros')     
    })
  })
})

// Buscar cargo função exibindo a opção de outros
Quando('envio uma requisição GET em cargos com true', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CargoFuncao?exibirOpcaoOutros=true`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna todos cargo função exibindo a opção de outros com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array')

    response.body.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item.id).to.be.a('number').and.greaterThan(0)
      expect(item).to.have.property('nome').and.to.be.a('string')
      expect(item).to.have.property('tipo')
      expect(item).to.have.property('outros')     
    })
  })
})

// Buscar cargo função não exibindo a opção de outros
Quando('envio uma requisição GET em cargos com false', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CargoFuncao?exibirOpcaoOutros=true`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna todos cargo função não exibindo a opção de outros com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array')

    response.body.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item.id).to.be.a('number').and.greaterThan(0)
      expect(item).to.have.property('nome').and.to.be.a('string')
      expect(item).to.have.property('tipo')
      expect(item).to.have.property('outros')     
    })
  })
})

// Não buscar cargo função sem autenticação
Quando('tento a requisição GET no endpoint de cargos', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CargoFuncao`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem todos cargo função', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar cargo função do tipo 1
Quando('envio uma requisição GET em cargos de tipo', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CargoFuncao/tipo/1`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna cargo função do tipo 1 com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array')

    response.body.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item.id).to.be.a('number').and.greaterThan(0)
      expect(item).to.have.property('nome').and.to.be.a('string')
      expect(item).to.have.property('tipo')
      expect(item).to.have.property('outros')     
    })
  })
})

// Buscar cargo função do tipo 2
Quando('envio uma requisição GET em cargos do tipo', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CargoFuncao/tipo/2`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna cargo função do tipo 2 com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array')

    response.body.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item.id).to.be.a('number').and.greaterThan(0)
      expect(item).to.have.property('nome').and.to.be.a('string')
      expect(item).to.have.property('tipo')
      expect(item).to.have.property('outros')     
    })
  })
})

// Buscar cargo função do tipo 3
Quando('envio uma requisição GET em cargos tipo', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CargoFuncao/tipo/3`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna cargo função do tipo 3 com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array')

    response.body.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item.id).to.be.a('number').and.greaterThan(0)
      expect(item).to.have.property('nome').and.to.be.a('string')
      expect(item).to.have.property('tipo')
      expect(item).to.have.property('outros')     
    })
  })
})

// Tipo é obrigatório em cargo função
Quando('envio uma requisição GET em cargos sem tipo', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CargoFuncao/tipo/`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna que tipo é obrigatório em cargo função com status 404', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(404)
  })
})

// Buscar cargo função tipo exibindo a opção de outros
Quando('envio uma requisição GET em cargos tipo com true', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CargoFuncao/tipo/1?exibirOpcaoOutros=true`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna todos cargo função tipo exibindo a opção de outros com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array').and.not.be.empty

    response.body.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item.id).to.be.a('number').and.greaterThan(0)
      expect(item).to.have.property('nome').and.to.be.a('string').and.not.be.empty
      expect(item).to.have.property('tipo')
      expect(item).to.have.property('outros')     
    })
  })
})

// Buscar cargo função tipo não exibindo a opção de outros
Quando('envio uma requisição GET em cargos tipo com false', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CargoFuncao/tipo/1?exibirOpcaoOutros=false`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna todos cargo função tipo não exibindo a opção de outros com status 200', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array').and.not.be.empty

    response.body.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item.id).to.be.a('number').and.greaterThan(0)
      expect(item).to.have.property('nome').and.to.be.a('string').and.not.be.empty
      expect(item).to.have.property('tipo')
      expect(item).to.have.property('outros')     
    })
  })
})

// Não buscar cargo função tipo sem autenticação
Quando('tento a requisição GET no endpoint de cargos tipos', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CargoFuncao/tipo/1`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem todos cargo função tipo', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})