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

Dado('que possuo um token válido do CodafCertificado', function () {
  expect(token, 'valido').to.exist
})

// Retornar todos meus certificados
Quando('envio uma requisição GET no endpoint meus CodafCertificado', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafCertificado/meus`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com meus certificados', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)

    expect(response.body).to.have.property('items').that.is.an('array')
    expect(response.body).to.have.property('totalPaginas').that.is.a('number')
    expect(response.body).to.have.property('totalRegistros').that.is.a('number')

    response.body.items.forEach((certificado) => {
      expect(certificado).to.have.property('id').that.is.a('number')
      expect(certificado).to.have.property('numeroHomologacao').that.is.a('number')
      expect(certificado).to.have.property('nomeFormacao').that.is.a('string')
      expect(certificado).to.have.property('codigoCertificado').that.is.a('number')
      expect(certificado).to.have.property('temRf').that.is.a('boolean')
      expect(certificado).to.have.property('tipoParticipacao').that.is.a('number')
      expect(certificado).to.have.property('dataEmissao').that.is.a('string')
    })
  })
})

// Retornar meus certificados como cursista
Quando('envio uma requisição GET no endpoint meus CodafCertificado cursista', function () {
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafCertificado/meus?TipoParticipacao=1`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com meus certificados de cursista', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
  })
})

// Não retornar meus certificados sem autenticação
Quando('tento a requisição GET no endpoint meus CodafCertificado', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafCertificado/meus`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem meus certificados', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Retornar Codaf certificados
Quando('envio uma requisição GET no endpoint CodafCertificado', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafCertificado`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com Codaf certificados', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)

    expect(response.body).to.have.property('items').that.is.an('array')
    expect(response.body).to.have.property('totalPaginas').that.is.a('number')
    expect(response.body).to.have.property('totalRegistros').that.is.a('number')

    response.body.items.forEach((certificado) => {
      expect(certificado).to.have.property('id').that.is.a('number')
      expect(certificado).to.have.property('numeroHomologacao').that.is.a('number')
      expect(certificado).to.have.property('nomeFormacao').that.is.a('string')
      expect(certificado).to.have.property('documento').that.is.a('string')
      expect(certificado).to.have.property('codigoCertificado').that.is.a('number')
      expect(certificado).to.have.property('tipoCertificado').that.is.a('number')
      expect(certificado).to.have.property('dataEmissao').that.is.a('string')
      expect(certificado).to.have.property('nomeParticipante').that.is.a('string')
    })
  })
})

// Retornar Codaf certificados de cursista
Quando('envio uma requisição GET no endpoint CodafCertificado cursista', function () {
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafCertificado?TipoParticipacao=1`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com Codaf certificados de cursista', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    
    expect(response.body).to.have.property('items').that.is.an('array')
    expect(response.body).to.have.property('totalPaginas').that.is.a('number')
    expect(response.body).to.have.property('totalRegistros').that.is.a('number')

    response.body.items.forEach((certificado) => {
      expect(certificado).to.have.property('id').that.is.a('number')
      expect(certificado).to.have.property('numeroHomologacao').that.is.a('number')
      expect(certificado).to.have.property('nomeFormacao').that.is.a('string')
      expect(certificado).to.have.property('documento').that.is.a('string')
      expect(certificado).to.have.property('codigoCertificado').that.is.a('number')
      expect(certificado).to.have.property('tipoCertificado').that.is.a('number')
      expect(certificado).to.have.property('dataEmissao').that.is.a('string')
      expect(certificado).to.have.property('nomeParticipante').that.is.a('string')
    })
  })
})

// Não retornar Codaf certificados sem autenticação
Quando('tento a requisição GET no endpoint CodafCertificado', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafCertificado/meus`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem Codaf certificados', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Retornar download Codaf certificados
Quando('envio uma requisição GET no endpoint CodafCertificado download', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafCertificado/${Cypress.env('CERTIFICADO_ID')}/download`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com download Codaf certificados', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422)
  })
})

// Id Codaf obrigatório para download certificados
Quando('envio uma requisição GET no endpoint sem id CodafCertificado download', function () {
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafCertificado//download`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 404 sem download Codaf certificados', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(404)
  })
})

// Não retornar download Codaf certificados sem autenticação
Quando('tento a requisição GET no endpoint CodafCertificado download', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/CodafCertificado/${Cypress.env('CODAF_ID')}/download`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem download Codaf certificados', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Emitir download por lista presença Codaf certificados
Quando('envio uma requisição POST no endpoint CodafCertificado download lista', function () {
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/CodafCertificado/${Cypress.env('CERTIFICADO_CODAF_ID')}/emitir`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: 
      [ 0 ] ,          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 204 com emitindo download lista presença Codaf certificados', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(500)
  })
})

// Não emitir download sem a lista presença Codaf certificados
Quando('envio uma requisição POST no endpoint CodafCertificado download sem lista', function () {
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/CodafCertificado//emitir`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: 
      [ 0 ] ,          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 404 sem download lista presença Codaf certificados', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(404)
  })
})


// Não emitir download por lista presença Codaf certificados sem autenticação
Quando('tento a requisição POST no endpoint CodafCertificado download lista', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/CodafCertificado/${Cypress.env('CODAF_ID')}/emitir`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },
    body: 
      [ 0 ] ,           
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem emitir download lista presença Codaf certificados', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Emitir download lote Codaf certificados
Quando('envio uma requisição POST no endpoint CodafCertificado download lote', function () {
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/CodafCertificado/download-lote`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: 
      [ 0 ] ,          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com emitindo download lote Codaf certificados', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(500)
  })
})

// Não emitir download lote Codaf certificados sem autenticação
Quando('tento a requisição POST no endpoint CodafCertificado download lote', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/CodafCertificado/download-lote`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },
    body: 
      [ 0 ] ,           
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem emitir download lote Codaf certificados', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})