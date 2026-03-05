import { Given, When, Then, Before } from "@badeball/cypress-cucumber-preprocessor"

let token

Before(() => {
  cy.gerar_token().then((token_valido) => {
    token = token_valido
  })
})

Given('que possuo um token de acesso', function () {
  expect(token, 'valido').to.exist
})

Given('que não possuo um token de acesso', function () { 
})

// Buscar dados do ano turma da modalidade 1
When('envio uma requisição GET com ano letivo da modalidade 1', function () { 

  const anoAtual = new Date().getFullYear()

  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AnoTurma?AnoLetivo=${anoAtual}&Modalidade=1`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    }, 
    timeout: 3000,          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com dados do ano turma da modalidade 1', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)    
  })
})

// Buscar dados do ano turma da modalidade 3
When('envio uma requisição GET com ano letivo da modalidade 3', function () {

  const anoAtual = new Date().getFullYear()

  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AnoTurma?AnoLetivo=${anoAtual}&Modalidade=3`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    }, 
    timeout: 3000,          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com dados do ano turma da modalidade 3', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)    
  })
})

// Buscar dados do ano turma da modalidade 4
When('envio uma requisição GET com ano letivo da modalidade 4', function () { 

  const anoAtual = new Date().getFullYear()

  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AnoTurma?AnoLetivo=${anoAtual}&Modalidade=4`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    }, 
    timeout: 3000,          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com dados do ano turma da modalidade 4', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)    
  })
})

// Buscar dados do ano turma da modalidade 5
When('envio uma requisição GET com ano letivo da modalidade 5', function () {
  
  const anoAtual = new Date().getFullYear()

  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AnoTurma?AnoLetivo=${anoAtual}&Modalidade=5`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    }, 
    timeout: 3000,          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com dados do ano turma da modalidade 5', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)    
  })
})

// Buscar dados do ano turma da modalidade 6
When('envio uma requisição GET com ano letivo da modalidade 6', function () { 

  const anoAtual = new Date().getFullYear()

  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AnoTurma?AnoLetivo=${anoAtual}&Modalidade=6`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    }, 
    timeout: 3000,          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com dados do ano turma da modalidade 6', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)    
  })
})

// Buscar dados do ano turma da modalidade 7
When('envio uma requisição GET com ano letivo da modalidade 7', function () {
  
  const anoAtual = new Date().getFullYear()

  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AnoTurma?AnoLetivo=${anoAtual}&Modalidade=7`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    }, 
    timeout: 3000,          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com dados do ano turma da modalidade 7', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)    
  })
})

// Buscar dados do ano turma da modalidade 8
When('envio uma requisição GET com ano letivo da modalidade 8', function () { 

  const anoAtual = new Date().getFullYear()

  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AnoTurma?AnoLetivo=${anoAtual}&Modalidade=8`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    }, 
    timeout: 3000,          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com dados do ano turma da modalidade 8', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)    
  })
})

// Buscar dados do ano turma da modalidade 9
When('envio uma requisição GET com ano letivo da modalidade 9', function () { 

  const anoAtual = new Date().getFullYear()

  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AnoTurma?AnoLetivo=${anoAtual}&Modalidade=9`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    }, 
    timeout: 3000,          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com dados do ano turma da modalidade 9', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)    
  })
})

// Buscar dados do ano turma da modalidade 10
When('envio uma requisição GET com ano letivo da modalidade 10', function () {
  
  const anoAtual = new Date().getFullYear()

  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AnoTurma?AnoLetivo=${anoAtual}&Modalidade=10`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    }, 
    timeout: 3000,          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 200 com dados do ano turma da modalidade 10', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)    
  })
})

// Não retorna dados de ano turma com modalidade inválida
When('envio uma requisição GET com ano letivo da modalidade inválida', function () {

  const anoAtual = new Date().getFullYear()

  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AnoTurma?AnoLetivo=${anoAtual}&Modalidade=11`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    }, 
    timeout: 3000,          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 422 sem dados do ano turma da modalidade', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422)    
  })
})

// Não retorna dados de ano turma sem modalidade
When('envio uma requisição GET com ano letivo sem modalidade', function () {

  const anoAtual = new Date().getFullYear()

  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AnoTurma?AnoLetivo=${anoAtual}&Modalidade=`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    }, 
    timeout: 3000,          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 422 sem dados do ano turma de modalidade', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422)    
  })
})

// Não retorna dados de ano turma sem ano letivo
When('envio uma requisição GET com ano letivo sem ano letivo', function () {

  const anoAtual = new Date().getFullYear()

  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AnoTurma?AnoLetivo=&Modalidade=1`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    }, 
    timeout: 3000,          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 500 sem dados do ano turma modalidade', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(500)    
  })
})

// Não buscar dados do ano turma da modalidade sem autenticação
When('tento a requisição GET com ano letivo da modalidade', function () { 

  const anoAtual = new Date().getFullYear()

  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AnoTurma?AnoLetivo=${anoAtual}&Modalidade=1`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Then('retorna o status 401 sem dados do ano turma da modalidade', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})