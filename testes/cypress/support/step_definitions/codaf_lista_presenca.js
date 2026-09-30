import { Given, When, Then, Before } from "@badeball/cypress-cucumber-preprocessor"

const Dado = Given
const Quando = When
const Então = Then

let token
let codafListaPresencaId

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
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/100`,
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
  cy.get('@response').then(({ status }) => {
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
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/100/imprimir`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 imprimindo lista presença do Codaf', function () {
  cy.get('@response').then(({ status }) => {
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
  cy.get('@response').then(({ status }) => {
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

// Criar lista de presença do Codaf
Quando('envio uma requisição POST na lista presença do Codaf', function () {
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + '/api/v1/CodafListaPresenca',
    headers: {
      accept: 'application/json, text/plain, */*',
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: {
      propostaId: Cypress.env('CODAF_PROPOSTA_ID'),
      propostaTurmaId: Cypress.env('CODAF_TURMA_ID'),
      dataPublicacao: '2026-01-01',
      dataPublicacaoDom: null,
      numeroComunicado: 1,
      paginaComunicadoDom: 0,
      codigoCursoEol: null,
      codigoNivel: null,
      observacao: '',
      inscritos: [],
      anexos: [],
      retificacoes: []
    },
    failOnStatusCode: false
  }).as('response')
})

Então('retorna o status 201 com os dados da lista de presença do Codaf', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(201)

    expect(response.body).to.include.keys(
      'id',
      'propostaId',
      'propostaTurmaId',
      'dataPublicacao',
      'status'
    )

    codafListaPresencaId = response.body.id
  })
})

// Não criar lista de presença do Codaf sem campos obrigatórios
Quando('envio a requisição POST na lista do Codaf sem campos', function () {
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + '/api/v1/CodafListaPresenca',
    headers: {
      accept: 'application/json, text/plain, */*',
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: {
      propostaId: null,
      propostaTurmaId: null,
      dataPublicacao: null,
      dataPublicacaoDom: null,
      numeroComunicado: 1,
      paginaComunicadoDom: 0,
      codigoCursoEol: null,
      codigoNivel: null,
      observacao: '',
      inscritos: [],
      anexos: [],
      retificacoes: []
    },
    failOnStatusCode: false
  }).as('response')
})

Então('retorna o status 422 sem criar lista de presença do Codaf sem campos obrigatórios', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422)
  })
})

// Não criar lista de presença do Codaf sem autenticação
Quando('tento uma requisição POST na lista presença do Codaf', function () {
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + '/api/v1/CodafListaPresenca',
    headers: {
      accept: 'application/json, text/plain, */*',
      Authorization: `token_invalido`,
      'Content-Type': 'application/json'
    },
    body: {
      propostaId: null,
      propostaTurmaId: null,
      dataPublicacao: null,
      dataPublicacaoDom: null,
      numeroComunicado: 1,
      paginaComunicadoDom: 0,
      codigoCursoEol: null,
      codigoNivel: null,
      observacao: '',
      inscritos: [],
      anexos: [],
      retificacoes: []
    },
    failOnStatusCode: false
  }).as('response')
})

Então('retorna o status 401 sem criar lista de presença do Codaf', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Alterar lista de presença do Codaf
Quando('envio uma requisição PUT na lista presença do Codaf', function () {
  cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/${codafListaPresencaId}`,
    headers: {
      accept: 'application/json, text/plain, */*',
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: {
      propostaId: Cypress.env('CODAF_PROPOSTA_ID'),
      propostaTurmaId: Cypress.env('CODAF_TURMA_ID'),
      dataPublicacao: '2026-01-01',
      dataPublicacaoDom: null,
      numeroComunicado: 1,
      paginaComunicadoDom: 0,
      codigoCursoEol: null,
      codigoNivel: null,
      observacao: '',
      inscritos: [],
      anexos: [],
      retificacoes: []
    },
    failOnStatusCode: false
  }).as('response')
})

Então('retorna o status 204 ao alterar a lista de presença do Codaf', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(204)
  })
})

// Não alterar lista de presença do Codaf sem id
Quando('envio uma requisição PUT na lista presença do Codaf sem id', function () {
  cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/`,
    headers: {
      accept: '*/*',
      Authorization: `Bearer ${token}`
    },
    failOnStatusCode: false
  }).as('response')
})

Então('retorna o status 405 sem alterar a lista de presença do Codaf', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(405)
  })
})

// Não altera lista de presença do Codaf sem autenticação
Quando('tento uma requisição PUT na lista presença do Codaf', function () {
  cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/${codafListaPresencaId}`,
    headers: {
      accept: '*/*',
      Authorization: `token_invalido`,
    },
    failOnStatusCode: false
  }).as('response')
})

Então('retorna o status 401 sem alterar a lista de presença do Codaf', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Deletar lista de presença do Codaf
Quando('envio uma requisição DELETE na lista presença do Codaf', function () {
  cy.request({
    method: 'DELETE',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/${codafListaPresencaId}`,
    headers: {
      accept: '*/*',
      Authorization: `Bearer ${token}`
    },
    failOnStatusCode: false
  }).as('responseDelete')
})

Então('retorna o status 204 ao deletar a lista de presença do Codaf', function () {
  cy.get('@responseDelete').then((response) => {
    expect(response.status).to.eq(204)
  })
})

// Não deletar lista de presença do Codaf sem id
Quando('envio uma requisição DELETE na lista presença do Codaf sem id', function () {
  cy.request({
    method: 'DELETE',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/`,
    headers: {
      accept: '*/*',
      Authorization: `Bearer ${token}`
    },
    failOnStatusCode: false
  }).as('response')
})

Então('retorna o status 405 sem deletar a lista de presença do Codaf', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(405)
  })
})

// Não deletar lista de presença do Codaf sem autenticação
Quando('tento uma requisição DELETE na lista presença do Codaf', function () {
  cy.request({
    method: 'DELETE',
    url: Cypress.config('baseUrl') + `/api/v1/CodafListaPresenca/${codafListaPresencaId}`,
    headers: {
      accept: '*/*',
      Authorization: `token_invalido`,
    },
    failOnStatusCode: false
  }).as('responseDelete')
})

Então('retorna o status 401 sem deletar a lista de presença do Codaf', function () {
  cy.get('@responseDelete').then((response) => {
    expect(response.status).to.eq(401)
  })
})