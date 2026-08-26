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

Dado('que possuo um token válido no endpoint CodafListaPresenca', function () {
  expect(token, 'valido').to.exist
})

// Buscar dados de presença do Codaf
Quando('envio uma requisição GET na lista presença do Codaf', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com dados de presença do Codaf', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)

    expect(response.body).to.have.property('items')
    expect(response.body).to.have.property('totalPaginas')
    expect(response.body).to.have.property('totalRegistros')

    expect(response.body.items).to.be.an('array')

    if (response.body.items.length > 0) {
      const item = response.body.items[0]

      expect(item).to.include.keys(
        'id',
        'numeroHomologacao',
        'nomeFormacao',
        'codigoFormacao',
        'nomeTurma',
        'nomeAreaPromotora',
        'status',
        'statusCertificacaoTurma',
        'codigoCursoEol',
        'codigoNivel'
      )
    }
  })
})

// Não buscar dados de presença do Codaf sem autenticação
Quando('tento a requisição GET na lista presença do Codaf', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem dados de presença do Codaf', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar dados por id da presença do Codaf
Quando('envio uma requisição GET id lista presença do Codaf', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/198`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com dados por id de presença do Codaf', function () {
  cy.get('@response').then(({ status, body }) => {
    expect(status).to.eq(200)

    expect(body).to.include.all.keys(
      'id',
      'propostaId',
      'propostaTurmaId',
      'nomeFormacao',
      'numeroHomologacao',
      'retificacoes',
      'anexos',
      'deltaInscritos',
      'comentario'
    )

    expect(body.retificacoes).to.be.an('array')
    expect(body.anexos).to.be.an('array')
    expect(body.deltaInscritos)

    expect(body).to.have.property('comentario')
  })
})

// Não buscar dados por id inválido na presença do Codaf
Quando('envio uma requisição GET id inválido lista presença do Codaf', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/0`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 404 sem dados por id de presença do Codaf', function () {
  cy.get('@response').then(({ status, body }) => {
    expect(status).to.eq(422)
  })
})

// Não buscar dados por id da presença do Codaf sem autenticação
Quando('tento a requisição GET id lista presença do Codaf', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem dados por id de presença do Codaf', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Imprimir lista presença do Codaf
Quando('envio uma requisição POST de imprimir lista presença do Codaf', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/198/imprimir`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 imprimindo lista presença do Codaf', function () {
  cy.get('@response').then(({ status, body }) => {
    expect(status).to.eq(200)
  })
})

// Não imprimir lista presença do Codaf sem id
Quando('envio sem id na requisição POST de imprimir lista do Codaf', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/0/imprimir`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 404 sem dados sem imprimir lista presença do Codaf', function () {
  cy.get('@response').then(({ status, body }) => {
    expect(status).to.eq(422)
  })
})

// Não imprimir lista presença do Codaf sem autenticação
Quando('tento a requisição POST de imprimir lista do Codaf', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/${Cypress.env('CERTIFICADO_CODAF_ID')}/imprimir`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem imprimir lista presença do Codaf', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})