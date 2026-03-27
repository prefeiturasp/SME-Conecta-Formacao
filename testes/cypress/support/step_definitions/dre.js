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

Dado('que possuo um token válido', function () {
  expect(token, 'valido').to.exist
})

Dado('que não possuo um token válido', function () { 
})

// Buscar cadastros de Dre
Quando('envio uma requisição GET no endpoint Dre', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Dre`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com todos cadastros de Dre', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200) 
  })
})

// Buscar cadastros de Dre exibindo todas
Quando('envio uma requisição GET no endpoint Dre como true', function () {
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Dre?exibirOpcaoTodos=true`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com dados de Dre exibindo todas', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
  })
})

// Buscar cadastros de Dre não exibindo todas
Quando('envio uma requisição GET no endpoint Dre como false', function () {
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Dre?exibirOpcaoTodos=false`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com dados de Dre não exibindo todas', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
  })
})

// Não buscar cadastros de Dre sem autenticação 
Quando('tento a requisição GET no endpoint Dre', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Dre`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem cadastros de Dre', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})