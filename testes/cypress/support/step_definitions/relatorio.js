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

Dado('que possuo um token válido no endpoint Relatorio', function () {
  expect(token, 'valido').to.exist
})

// Gerar relatório de inscritos por formação
Quando('envio uma requisição POST em inscritos por formação', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/Relatorio/inscritos-por-formacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: {
      periodoDeRealizacaoInicial: "2026-01-01T18:47:33.143Z",
      periodoDeRealizacaoFinal: "2026-08-01T18:47:33.143Z",
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 202 gerando relatório de inscritos por formação', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(202)    
  })
})

// Período obrigatório no relatório de inscritos por formação
Quando('envio uma requisição POST sem período no relatório por formação', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/Relatorio/inscritos-por-formacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: {
      periodoDeRealizacaoInicial: " ",
      periodoDeRealizacaoFinal: " ",
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 422 gerando relatório de inscritos por formação', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422)    
  })
})

// Não gerar relatório de inscritos por formação sem autenticação
Quando('tento a requisição POST em inscritos por formação', function () { 
    return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/Relatorio/inscritos-por-formacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },
    body: {
      periodoDeRealizacaoInicial: "2026-01-01T18:47:33.143Z",
      periodoDeRealizacaoFinal: "2026-08-01T18:47:33.143Z",
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem relatório de inscritos por formação', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})