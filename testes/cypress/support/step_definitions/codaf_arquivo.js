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

Dado('que possuo um token válido no endpoint CodafArquivo', function () {
  expect(token, 'valido').to.exist
})

// Buscar o modelo do termo do Codaf
Quando('envio uma requisição GET no Codaf lista presença', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/termo-responsabilidade/modelo`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com modelo do termo do Codaf', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
  })
})

// Não buscar modelo do termo do Codaf sem autenticação
Quando('tento a requisição GET no Codaf lista presença', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/termo-responsabilidade/modelo`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem modelo do termo do Codaf', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})