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

Dado('que possuo um token válido no endpoint UsuarioRedeParceria', function () {
  expect(token, 'valido').to.exist
})

// Buscar usuários rede parceria
Quando('envio uma requisição GET buscar o usuário de parceria', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/UsuarioRedeParceria`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com usuários rede parceria', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)

    expect(response.body).to.be.an('object')
    expect(response.body).to.have.property('items')
    expect(response.body.items).to.be.an('array')

    expect(response.body.items[0]).to.have.property('nome')
    expect(response.body.items[0]).to.have.property('cpf')
    expect(response.body.items[0]).to.have.property('telefone')
    expect(response.body.items[0]).to.have.property('email')
    expect(response.body.items[0]).to.have.property('situacao')    
  })
})

// Não buscar usuários rede parceria sem autenticação
Quando('tento a requisição GET buscar o usuário de parceria', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/UsuarioRedeParceria`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem usuários rede parceria', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar situação de usuários rede parceria
Quando('envio uma requisição GET da situação do usuário parceria', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/UsuarioRedeParceria/situacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com situação de usuários rede parceria', function () {
  cy.get('@response').then((response) => {

    expect(response.status).to.eq(200)

    expect(response.body).to.be.an('array')

    expect(response.body[0]).to.have.property('id')
    expect(response.body[0]).to.have.property('descricao')

    cy.log(`ID: ${response.body[0].id}`)
    cy.log(`Descrição: ${response.body[0].descricao}`)
  })
})

// Não buscar situação de usuários rede parceria sem autenticação
Quando('tento a requisição GET da situação de usuários parceria', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/UsuarioRedeParceria/situacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem situação de usuários rede parceria', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar id de usuários rede parceria
Quando('envio uma requisição GET id do usuário parceria', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/UsuarioRedeParceria/56280`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com id de usuários rede parceria', function () {
  cy.get('@response').then((response) => {

    expect(response.status).to.eq(200)

    expect(response.body).to.be.an('object')

    expect(response.body).to.have.property('areaPromotoraId')
    expect(response.body).to.have.property('nome')
    expect(response.body).to.have.property('cpf')
    expect(response.body).to.have.property('email')
    expect(response.body).to.have.property('telefone')
    expect(response.body).to.have.property('situacao')
  })
})

// Id de usuários rede parceria inválido
Quando('envio uma requisição GET sem id do usuário parceria', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/UsuarioRedeParceria/0`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 400 que id de usuários rede parceria é inválido', function () {
  cy.get('@response').then((response) => {

    expect(response.status).to.eq(400)
  })
})

// Não buscar id de usuários rede parceria sem autenticação
Quando('tento a requisição GET id de usuários parceria', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/UsuarioRedeParceria/1`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem id de usuários rede parceria', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})