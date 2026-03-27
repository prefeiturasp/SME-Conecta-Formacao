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

Dado('que possuo um token válido no endpoint Modalidade', function () {
  expect(token, 'valido').to.exist
})

// Buscar cadastros de modalidades
Quando('envio uma requisição GET buscar modalidades', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Modalidade`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com cadastros de modalidades', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200) 
  })
})

// Não buscar cadastros de modalidades sem autenticação
Quando('tento a requisição GET buscar modalidades', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Modalidade`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem cadastros de modalidades', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})