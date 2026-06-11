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

Dado('que possuo um token válido no endpoint Funcionario', function () {
  expect(token, 'valido').to.exist
})

// Buscar funcionários com usuários admin df
Quando('envio uma requisição GET obter usuarios admin df', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Funcionario/obter-usuarios-admin-df`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 funcionários com usuários admin df', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200) 
    response.body.forEach((usuario) => {
      expect(usuario).to.have.property('nome')
      expect(usuario).to.have.property('login')
    })
  })
})

// Não buscar funcionários com usuários admin df sem autenticação
Quando('tento a requisição GET obter usuarios admin df', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Funcionario/obter-usuarios-admin-df`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem funcionários com usuários admin df', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar funcionários com usuários parcerista
Quando('envio uma requisição GET obter usuarios parcerista', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Funcionario/obter-parecerista`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 funcionários com usuários parcerista', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200) 
    response.body.forEach((usuario) => {
      expect(usuario).to.have.property('nome')
      expect(usuario).to.have.property('login')
    })
  })
})

// Não buscar funcionários com usuários parcerista sem autenticação
Quando('tento a requisição GET obter usuarios parcerista', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Funcionario/obter-parecerista`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem funcionários com usuários parcerista', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})