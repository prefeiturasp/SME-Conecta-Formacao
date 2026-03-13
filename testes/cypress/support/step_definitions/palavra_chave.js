import { Given, When, Then, Before } from "@badeball/cypress-cucumber-preprocessor"

let token

Before(() => {
  cy.gerar_token().then((token_valido) => {
    token = token_valido
  })
})

Given('que possuo um token válido no endpoint PalavraChave', function () {
  expect(token, 'valido').to.exist
})

// Buscar cadastros de palavra chave
When('envio uma requisição GET buscar palavra chave', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/PalavraChave`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com cadastros de palavra chave', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200) 
  })
})

// Não buscar cadastros de palavra chave sem autenticação
When('tento a requisição GET buscar palavra chave', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/PalavraChave`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 401 sem palavra chave', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})