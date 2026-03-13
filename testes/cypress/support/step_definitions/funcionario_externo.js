import { Given, When, Then, Before } from "@badeball/cypress-cucumber-preprocessor"

let token

Before(() => {
  cy.gerar_token().then((token_valido) => {
    token = token_valido
  })
})

Given('que possuo um token válido no endpoint FuncionarioExterno', function () {
  expect(token, 'valido').to.exist
})

// Buscar funcionário externo
When('envio uma requisição GET obter usuarios externos', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/FuncionarioExterno/${Cypress.env('LOGIN_EXTERNO')}`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com funcionário externo', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.have.property('nomePessoa')
    expect(response.body).to.have.property('cpf')
    expect(response.body).to.have.property('codigoUE')
    expect(response.body).to.have.property('nomeUe')
    expect(response.body).to.have.property('ues')
  })
})

// Não buscar funcionário externo sem documento
When('envio uma requisição GET obter usuarios externos sem documento', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/FuncionarioExterno/`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Then('não busca funcionário externo', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(404) 
  })
})

// Não buscar funcionário externo inválido
When('tento a requisição GET obter usuarios externos inexistente', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/FuncionarioExterno/1234567890`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 204 sem funcionário externo inválido', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(204)
  })
})