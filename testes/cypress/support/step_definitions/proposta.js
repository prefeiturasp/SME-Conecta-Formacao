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

Dado('que possuo um token válido no endpoint Proposta', function () {
  expect(token, 'valido').to.exist
})

// Buscar informações do cadastrante
Quando('envio uma requisição GET na informações cadastrante', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/informacoes-cadastrante`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com informações do cadastrante', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.have.property('usuarioLogadoNome')
    expect(response.body).to.have.property('usuarioLogadoEmail')
    expect(response.body).to.have.property('areaPromotora')
    expect(response.body).to.have.property('areaPromotoraTipo')
    expect(response.body).to.have.property('areaPromotoraTipoId')
    expect(response.body).to.have.property('areaPromotoraTelefones')
    expect(response.body).to.have.property('areaPromotoraEmails')
  })
})

// Não buscar informações do cadastrante sem autenticação
Quando('tento a requisição GET na informações cadastrante', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/informacoes-cadastrante`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem informações do cadastrante', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar roteiro
Quando('envio uma requisição GET roteiro', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/roteiro`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com roteiro', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(204)
  })
})

// Não buscar roteiro sem autenticação
Quando('tento a requisição GET roteiro', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/roteiro`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem roteiro', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar critério validação de inscrição 
Quando('envio uma requisição GET criterio validacao inscricao', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/criterio-validacao-inscricao `,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com critério validação de inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array').and.not.be.empty
    expect(response.body[0]).to.have.all.keys(
      'id',
      'nome',
      'unico',
      'outros'
    )
  })
})

// Não buscar critério validação de inscrição sem autenticação
Quando('tento a requisição GET criterio validacao inscricao', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/criterio-validacao-inscricao`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem critério validação de inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar tipo de formação 
Quando('envio uma requisição GET tipo formacao', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/tipo-formacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com tipo de formação', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array').and.not.be.empty

    cy.wrap(response.body).each((item) => {
      expect(item).to.have.property('id')
      expect(item).to.have.property('descricao')
    })
  })
})

// Não buscar tipo de formação sem autenticação
Quando('tento a requisição GET tipo formacao', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/tipo-formacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem tipo de formação', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar tipo de inscrição 
Quando('envio uma requisição GET tipo inscricao', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/tipo-inscricao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com tipo de inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array').and.not.be.empty

    cy.wrap(response.body).each((item) => {
      expect(item).to.have.property('id')
      expect(item).to.have.property('descricao')
    })
  })
})

// Não buscar tipo de inscrição sem autenticação
Quando('tento a requisição GET tipo inscricao', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/tipo-inscricao`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem tipo de inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar situação
Quando('envio uma requisição GET situacao', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/situacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com situação', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array').and.not.be.empty

    cy.wrap(response.body).each((item) => {
      expect(item).to.have.property('id')
      expect(item).to.have.property('descricao')
    })
  })
})

// Não buscar situação sem autenticação
Quando('tento a requisição GET situacao', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/situacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem situação', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar tipo encontro
Quando('envio uma requisição GET tipo encontro', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/tipo-encontro`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com tipo encontro', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array').and.not.be.empty

    cy.wrap(response.body).each((item) => {
      expect(item).to.have.property('id')
      expect(item).to.have.property('descricao')
    })
  })
})

// Não buscar tipo encontro sem autenticação
Quando('tento a requisição GET tipo encontro', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/tipo-encontro`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem tipo encontro', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar formação homologada
Quando('envio uma requisição GET formacao homologada', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/formacao-homologada`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com formação homologada', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array').and.not.be.empty

    cy.wrap(response.body).each((item) => {
      expect(item).to.have.property('id')
      expect(item).to.have.property('descricao')
    })
  })
})

// Não buscar formação homologada sem autenticação
Quando('tento a requisição GET formacao homologada', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/formacao-homologada`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem formação homologada', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar proposta
Quando('envio uma requisição GET proposta', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com proposta', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200) 
  })
})

// Não buscar proposta sem autenticação
Quando('tento a requisição GET proposta', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/proposta`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem proposta', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar dashboard
Quando('envio uma requisição GET dashboard', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/dashboard`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com dashboard', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array').and.not.be.empty

    cy.wrap(response.body).each((item) => {
      expect(item).to.have.property('situacao')
      expect(item).to.have.property('cor')
      expect(item).to.have.property('totalRegistros')
    })
  })
})

// Não buscar dashboard sem autenticação
Quando('tento a requisição GET dashboard', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/dashboard`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem dashboard', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar horas totais proposta
Quando('envio uma requisição GET horas totais proposta', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/horas-totais-proposta`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com horas totais proposta', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array').and.not.be.empty

    cy.wrap(response.body).each((item) => {
      expect(item).to.have.property('id')
      expect(item).to.have.property('descricao')
    })
  })
})

// Não buscar horas totais proposta sem autenticação
Quando('tento a requisição GET horas totais proposta', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/horas-totais-proposta`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem horas totais proposta', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar autocompletar formação
Quando('envio uma requisição GET autocompletar formacao', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/autocompletar-formacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com autocompletar formação', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)  
  })
})

// Não buscar autocompletar formação sem autenticação
Quando('tento a requisição GET autocompletar formacao', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Proposta/autocompletar-formacao`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem autocompletar formação', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})