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

Dado('que possuo um token válido no endpoint Inscricao', function () {
  expect(token, 'valido').to.exist
})

// Buscar dados de inscrição
Quando('envio uma requisição GET dos dados de inscrição', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/dados-inscricao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com dados de inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('object')
    expect(response.body).to.have.property('usuarioNome')
    expect(response.body).to.have.property('usuarioRf')
    expect(response.body).to.have.property('usuarioCpf')
    expect(response.body).to.have.property('usuarioEmail')
    expect(response.body).to.have.property('usuarioCargos')
  })
})

// Não buscar dados de inscrição sem autenticação
Quando('tento a requisição GET dos dados de inscrição', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/dados-inscricao`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem dados de inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar inscrição
Quando('envio uma requisição GET na inscrição', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com a inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('object')
    expect(response.body).to.have.property('items')
    expect(response.body.items).to.be.an('array')

    response.body.items.forEach((item) => {
      expect(item).to.have.property('codigoFormacao')
      expect(item).to.have.property('nomeFormacao')
      expect(item).to.have.property('nomeTurma')
      expect(item).to.have.property('datas')
      expect(item).to.have.property('cargoFuncaoCodigo')
    })
  })
})

// Não buscar inscrição sem autenticação
Quando('tento a requisição GET na inscrição', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem a inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar próximas inscrições
Quando('envio uma requisição GET em próximas inscrições', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/proximas`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com próximas inscrições', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('object')
    expect(response.body).to.have.property('items')
    expect(response.body.items).to.be.an('array')

    response.body.items.forEach((item) => {
      expect(item).to.have.property('codigoFormacao')
      expect(item).to.have.property('nomeFormacao')
      expect(item).to.have.property('nomeTurma')
      expect(item).to.have.property('datas')
      expect(item).to.have.property('cargoFuncaoCodigo')
    })
  })
})

// Não buscar próximas inscrições sem autenticação
Quando('tento a requisição GET em próximas inscrições', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/proximas`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem próximas inscrições', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar inscrição finalizada
Quando('envio uma requisição GET em inscrição encerradas', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/finalizadas`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com inscrição finalizada', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('object')
    expect(response.body).to.have.property('items')
    expect(response.body.items).to.be.an('array')

    response.body.items.forEach((item) => {
      expect(item).to.have.property('codigoFormacao')
      expect(item).to.have.property('nomeFormacao')
      expect(item).to.have.property('nomeTurma')
      expect(item).to.have.property('datas')
      expect(item).to.have.property('cargoFuncaoCodigo')
    })
  })
})

// Não buscar inscrição finalizada sem autenticação
Quando('tento a requisição GET em inscrição encerradas', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/finalizadas`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem inscrição finalizada', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar formação de turmas
Quando('envio uma requisição GET em turma formadas', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/formacao-turmas`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com formação de turmas', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)    
  })
})

// Não buscar formação de turmas sem autenticação
Quando('tento a requisição GET em turma formadas', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/formacao-turmas`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem formação de turmas', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar tipos de inscrição
Quando('envio uma requisição GET em inscrição tipos', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/tipos`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com tipos de inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array')    
    expect(response.body[0]).to.have.property('id')
    expect(response.body[0]).to.have.property('descricao')
  })
})

// Não buscar tipos de inscrição sem autenticação
Quando('tento a requisição GET em inscrição tipos', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/tipos`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem tipos de inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar inscrição de cursista
Quando('envio uma requisição GET no cursista em inscrição', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/cursista?cpf=${Cypress.env('CPF')}`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com inscrição de cursista', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.not.be.null
    expect(response.body).to.be.an('object')
    expect(response.body).to.have.property('cpf')
    expect(response.body).to.have.property('nome')
  })
})

// Não buscar inscrição de cursista sem autenticação
Quando('tento a requisição GET no cursista em inscrição', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/cursista?cpf=${Cypress.env('CPF')}`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem inscrição de cursista', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar dados de inscrição através do id
Quando('envio uma requisição GET dados inscrição por proposta id', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/dados-inscricao-proposta/${Cypress.env('PROPOSTA_ID')}`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com dados de inscrição através do id', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)

    const body = response.body

    expect(body).to.be.an('object')
    expect(body).to.have.property('usuarioNome').that.is.a('string')
    expect(body).to.have.property('usuarioRf').that.is.a('string')
    expect(body).to.have.property('usuarioCpf').that.is.a('string')
    expect(body).to.have.property('usuarioEmail').that.is.a('string')
    expect(body).to.have.property('vagaRemanescente').that.is.a('boolean')
    expect(body).to.have.property('usuarioCargos').that.is.an('array')

    if (body.usuarioCargos.length > 0) {
      const cargo = body.usuarioCargos[0]

      expect(cargo).to.have.property('codigo').that.is.a('string')
      expect(cargo).to.have.property('descricao').that.is.a('string')
      expect(cargo).to.have.property('dreCodigo').that.is.a('string')
      expect(cargo).to.have.property('ueCodigo').that.is.a('string')
      expect(cargo).to.have.property('tipoVinculo').that.is.a('number')
      expect(cargo).to.have.property('funcoes').that.is.an('array')

      expect(cargo).to.have.property('dataInicio')
      expect(cargo.dataInicio === null || typeof cargo.dataInicio === 'string').to.eq(true)
    }
  })
})

// Não buscar dados de inscrição através sem id
Quando('envio uma requisição GET dados inscrição por proposta sem id', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/dados-inscricao-proposta/ `,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('não retorna o status 404 com dados de inscrição sem id', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(404)    
  })
})

// Não buscar dados de inscrição através do id sem autenticação
Quando('tento a requisição GET dados inscrição por proposta id', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/dados-inscricao-proposta/${Cypress.env('PROPOSTA_ID')}`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem dados de inscrição através do id', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar turmas através da proposta id
Quando('envio uma requisição GET turmas por propostaId', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/turmas/${Cypress.env('PROPOSTA_ID')}`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com turmas através da proposta id', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)

    const body = response.body

    expect(body).to.be.an('array')

    body.forEach((turma) => {
      expect(turma).to.be.an('object')
      expect(turma).to.have.property('id').that.is.a('number')
      expect(turma).to.have.property('descricao').that.is.a('string')
    })
  })
})

// Não buscar turmas através da proposta sem id
Quando('requisição GET turmas por proposta sem Id', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/turmas/ `,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('não retorna o status 404 turmas através da proposta sem id', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(404)    
  })
})

// Não buscar turmas através da proposta id sem autenticação
Quando('tento a requisição GET turmas por propostaId', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/turmas/${Cypress.env('PROPOSTA_ID')}`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem turmas através da proposta id', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar inscrição através da proposta id
Quando('envio uma requisição GET inscricao por propostaId', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/${Cypress.env('PROPOSTA_ID')}`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com inscrição através da proposta id', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)

    const body = response.body

    expect(body).to.be.an('object')
    expect(body).to.have.property('items').that.is.an('array')
    expect(body).to.have.property('totalPaginas').that.is.a('number')
    expect(body).to.have.property('totalRegistros').that.is.a('number')

    body.items.forEach((item) => {
      expect(item).to.be.an('object')

      expect(item).to.have.property('inscricaoId').that.is.a('number')
      expect(item).to.have.property('nomeTurma').that.is.a('string')
      expect(item).to.have.property('registroFuncional').that.is.a('string')
      expect(item).to.have.property('cpf').that.is.a('string')
      expect(item).to.have.property('nomeCursista').that.is.a('string')
      expect(item).to.have.property('cargoFuncao').that.is.a('string')
      expect(item).to.have.property('situacaoCodigo').that.is.a('number')
      expect(item).to.have.property('situacao').that.is.a('string')
      expect(item).to.have.property('origem').that.is.a('string')
      expect(item).to.have.property('integrarNoSga').that.is.a('boolean')
      expect(item).to.have.property('iniciado').that.is.a('boolean')

      expect(item).to.have.property('permissao').that.is.an('object')
      expect(item.permissao).to.have.property('podeCancelar').that.is.a('boolean')
      expect(item.permissao).to.have.property('podeColocarEmEspera').that.is.a('boolean')
      expect(item.permissao).to.have.property('podeConfirmar').that.is.a('boolean')
      expect(item.permissao).to.have.property('podeReativar').that.is.a('boolean')

      expect(item).to.have.property('dataInscricao').that.is.a('string')

      expect(item).to.have.property('anexos').that.is.an('array')

      item.anexos.forEach((anexo) => {
        expect(anexo).to.have.property('nome').that.is.a('string')
        expect(anexo).to.have.property('codigo').that.is.a('string')
      })
    })
  })
})

// Não buscar inscrição através da proposta sem id
Quando('requisição GET inscricao por proposta sem Id', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/ /`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('não retorna o status 404 inscrição através da proposta sem id', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(404)    
  })
})

// Não buscar inscrição através da proposta id sem autenticação
Quando('tento a requisição GET inscricao por propostaId', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/${Cypress.env('PROPOSTA_ID')}`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem inscrição através da proposta id', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar inscrição aberta através da proposta id
Quando('envio uma requisição GET inscricao aberta por propostaId', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/${Cypress.env('PROPOSTA_ID')}/abertas`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com inscrição abertas através da proposta id', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
  })
})

// Não buscar inscrição aberta através da proposta sem id
Quando('requisição GET inscricao aberta por proposta sem Id', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/ /abertas`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('não retorna o status 422 inscrição aberta através da proposta sem id', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422)    
  })
})

// Não buscar inscrição aberta através da proposta id sem autenticação
Quando('tento a requisição GET inscricao aberta por propostaId', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Inscricao/${Cypress.env('PROPOSTA_ID')}/abertas`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem inscrição aberta através da proposta id', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Cadastrar inscrição com sucesso
Quando('envio uma requisição POST para cadastrar inscrição', function () {
  return cy.request({
    method: 'POST',
    url: `${Cypress.config('baseUrl')}/api/v1/Inscricao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: {
      propostaTurmaId: Cypress.env('PROPOSTA_TURMA_ID'),
      cargoCodigo: Cypress.env('CARGO_CODIGO') || '',
      cargoDreCodigo: Cypress.env('CARGO_DRE_CODIGO') || '',
      cargoUeCodigo: Cypress.env('CARGO_UE_CODIGO') || '',
      tipoVinculo: Cypress.env('TIPO_VINCULO'),
      vagaRemanescente: false,
      usuarioAcessibilidade: {
        possuiDeficiencia: false,
        salvar: true
      }
    },
    failOnStatusCode: false
  }).then((response) => {
  
    if (response.status === 200) {
      expect(response.body.entidadeId).to.exist

      return cy.request({
        method: 'PUT',
        url: `${Cypress.config('baseUrl')}/api/v1/Inscricao/${response.body.entidadeId}/cancelar`,
        headers: {
          accept: 'application/json',
          Authorization: `Bearer ${token}`
        },
        failOnStatusCode: false
      }).then((cancelResponse) => {
        cy.wrap(cancelResponse).as('response')
      })
    }
    expect(response.status, JSON.stringify(response.body)).to.eq(200)
  })
})

Então('retorna o status 200 com inscrição cadastrada com sucesso', function () {
  cy.get('@response').then((response) => {
    expect(response.status, JSON.stringify(response.body)).to.eq(200)
  })
})

// Não cadastrar inscrição sem propostaTurmaId
Quando('envio uma requisição POST para cadastrar inscrição sem propostaTurmaId', function () {
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + '/api/v1/Inscricao',
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
   body: {
      propostaTurmaId: '',     
      cargoCodigo: Cypress.env('CARGO_CODIGO') || '',
      cargoDreCodigo: Cypress.env('CARGO_DRE_CODIGO') || '',
      cargoUeCodigo: Cypress.env('CARGO_UE_CODIGO') || '',      
      tipoVinculo: Cypress.env('TIPO_VINCULO'),
      vagaRemanescente: false,
      usuarioAcessibilidade: {       
        possuiDeficiencia: false,    
        salvar: true
      }
    },
    failOnStatusCode: false
  }).as('response')
})

Então('retorna o status 422 ao tentar cadastrar inscrição sem propostaTurmaId', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422)
  })
})

// Não cadastrar inscrição sem autenticação
Quando('tento enviar uma requisição POST para cadastrar inscrição', function () {
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + '/api/v1/Inscricao',
    headers: {
      accept: 'text/plain',
      'Content-Type': 'application/json'
    },
    body: {
      propostaTurmaId: Cypress.env('PROPOSTA_TURMA_ID'),     
      cargoCodigo: Cypress.env('CARGO_CODIGO') || '',
      cargoDreCodigo: Cypress.env('CARGO_DRE_CODIGO') || '',
      cargoUeCodigo: Cypress.env('CARGO_UE_CODIGO') || '',      
      tipoVinculo: Cypress.env('TIPO_VINCULO'),
      vagaRemanescente: false,
      usuarioAcessibilidade: {       
        possuiDeficiencia: false,    
        salvar: true
      }
    },
    failOnStatusCode: false
  }).as('response')
})

Então('retorna o status 401 sem autenticação ao cadastrar inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Cancelar inscrição com sucesso
Quando('envio uma requisição PUT para cancelar inscrição', function () {
  return cy.request({
    method: 'POST',
    url: `${Cypress.config('baseUrl')}/api/v1/Inscricao`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: {
      propostaTurmaId: Cypress.env('PROPOSTA_TURMA_ID'),
      cargoCodigo: Cypress.env('CARGO_CODIGO') || '',
      cargoDreCodigo: Cypress.env('CARGO_DRE_CODIGO') || '',
      cargoUeCodigo: Cypress.env('CARGO_UE_CODIGO') || '',
      tipoVinculo: Cypress.env('TIPO_VINCULO'),
      vagaRemanescente: false,
      usuarioAcessibilidade: {
        possuiDeficiencia: false,
        salvar: true
      }
    },
    failOnStatusCode: false
  }).then((responsePost) => {
    expect(responsePost.status, JSON.stringify(responsePost.body)).to.eq(200)

    const inscricaoId = responsePost.body.entidadeId
    expect(inscricaoId).to.exist

    return cy.request({
      method: 'PUT',
      url: `${Cypress.config('baseUrl')}/api/v1/Inscricao/${inscricaoId}/cancelar`,
      headers: {
        accept: 'application/json',
        Authorization: `Bearer ${token}`
      },
      failOnStatusCode: false
    }).as('response')
  })
})

Então('retorna o status 200 com inscrição cancelada com sucesso', function () {
  cy.get('@response').then((response) => {
    expect(response.status, JSON.stringify(response.body)).to.eq(200)
  })
})

// Cancelar inscrição sem id
Quando('envio uma requisição PUT para cancelar inscrição sem id', function () {
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + '/api/v1/Inscricao//cancelar',
    headers: {
      accept: 'application/json, text/plain, */*',
      Authorization: `Bearer ${token}`
    },
    failOnStatusCode: false
  }).as('response')
})

Então('retorna o status 415 ao tentar cancelar inscrição sem id', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(415)
  })
})

// Cancelar inscrição sem autenticação
Quando('tento enviar uma requisição PUT para cancelar inscrição', function () {
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + '/api/v1/Inscricao',
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`,
      'Content-Type': 'application/json'
    },
    body: {
      propostaTurmaId: Cypress.env('PROPOSTA_TURMA_ID'),
      cargoCodigo: Cypress.env('CARGO_CODIGO') || '',
      cargoDreCodigo: Cypress.env('CARGO_DRE_CODIGO') || '',
      cargoUeCodigo: Cypress.env('CARGO_UE_CODIGO') || '',
      tipoVinculo: Cypress.env('TIPO_VINCULO'),
      vagaRemanescente: false,
      usuarioAcessibilidade: {
        possuiDeficiencia: false,
        salvar: true
      }
    },
    failOnStatusCode: false
  }).then((responsePost) => {
    expect(responsePost.status, JSON.stringify(responsePost.body)).to.eq(401)

    const inscricaoId = 1

    return cy.request({
      method: 'PUT',
      url: Cypress.config('baseUrl') + `/api/v1/Inscricao/${inscricaoId}/cancelar`,
      headers: {
        accept: 'application/json, text/plain, */*'
      },
      failOnStatusCode: false
    }).as('response')
  })
})

Então('retorna o status 401 sem autenticação ao cancelar inscrição', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Cadastrar inscrição manual com sucesso
Quando('envio uma requisição POST para cadastrar inscrição manual', function () {
  const payload = {
    propostaTurmaId: Number(Cypress.env('PROPOSTA_TURMA_ID')),
    profissionalRede: true,
    podeContinuar: true,
    registroFuncional: String(Cypress.env('LOGIN_ADM_GERAL')),
    cpf: String(Cypress.env('CPF')),
    cargoCodigo: String(Cypress.env('CARGO_CODIGO')),
    cargoDreCodigo: String(Cypress.env('CARGO_DRE_CODIGO')),
    cargoUeCodigo: String(Cypress.env('CARGO_UE_CODIGO')),
    tipoVinculo: Number(Cypress.env('TIPO_VINCULO'))
  }

  return cy.request({
    method: 'POST',
    url: `${Cypress.config('baseUrl')}/api/v1/Inscricao/manual`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: payload,
    failOnStatusCode: false
  }).then((response) => {

    cy.wrap(response).as('response')

    expect(response.status, JSON.stringify(response.body)).to.eq(200)
    expect(response.body.entidadeId).to.exist

    const inscricaoId = response.body.entidadeId

    return cy.request({
      method: 'PUT',
      url: `${Cypress.config('baseUrl')}/api/v1/Inscricao/${inscricaoId}/cancelar`,
      headers: {
        accept: 'application/json',
        Authorization: `Bearer ${token}`
      },
      failOnStatusCode: false
    }).then((cancelResponse) => {

      expect(cancelResponse.status).to.eq(200)
    })
  })
})

Então('retorna o status 200 com inscrição manual cadastrada com sucesso', function () {
  cy.get('@response').then((response) => {
    expect(response.status, JSON.stringify(response.body)).to.eq(200)
    expect(response.body.entidadeId).to.exist
  })
})

// Não cadastrar inscrição manual sem propostaTurmaId
Quando('envio uma requisição POST para cadastrar inscrição manual sem propostaTurmaId', function () {
  const payload = {
    propostaTurmaId: '',
    profissionalRede: true,
    podeContinuar: true,
    registroFuncional: String(Cypress.env('LOGIN_ADM_GERAL')),
    cpf: String(Cypress.env('CPF')),
    cargoCodigo: String(Cypress.env('CARGO_CODIGO')),
    cargoDreCodigo: String(Cypress.env('CARGO_DRE_CODIGO')),
    cargoUeCodigo: String(Cypress.env('CARGO_UE_CODIGO')),
    tipoVinculo: Number(Cypress.env('TIPO_VINCULO'))
  }

  return cy.request({
    method: 'POST',
    url: `${Cypress.config('baseUrl')}/api/v1/Inscricao/manual`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: payload,
    failOnStatusCode: false
  }).then((response) => {

    cy.wrap(response).as('response')

    expect(response.status, JSON.stringify(response.body)).to.eq(422)
    expect(response.body.entidadeId).not.exist

    const inscricaoId = 1

    return cy.request({
      method: 'PUT',
      url: `${Cypress.config('baseUrl')}/api/v1/Inscricao/${inscricaoId}/cancelar`,
      headers: {
        accept: 'application/json',
        Authorization: `Bearer ${token}`
      },
      failOnStatusCode: false
    }).then((response) => {

    })
  })
})

Então('retorna o status 422 da inscrição manual não cadastrada sem propostaTurmaId', function () {
  cy.get('@response').then((response) => {
    expect(response.status, JSON.stringify(response.body)).to.eq(422)
  })
})

// Não cadastrar inscrição manual sem autenticação
Quando('tento enviar uma requisição POST de inscrição manual', function () {
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + '/api/v1/Inscricao/manual',
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`,
      'Content-Type': 'application/json'
    },
    body: {
      propostaTurmaId: Cypress.env('PROPOSTA_TURMA_ID'),
      cargoCodigo: Cypress.env('CARGO_CODIGO') || '',
      cargoDreCodigo: Cypress.env('CARGO_DRE_CODIGO') || '',
      cargoUeCodigo: Cypress.env('CARGO_UE_CODIGO') || '',
      tipoVinculo: Cypress.env('TIPO_VINCULO'),
      vagaRemanescente: false,
      usuarioAcessibilidade: {
        possuiDeficiencia: false,
        salvar: true
      }
    },
    failOnStatusCode: false
  }).then((responsePost) => {
    expect(responsePost.status, JSON.stringify(responsePost.body)).to.eq(401)

    const inscricaoId = 1

    return cy.request({
      method: 'PUT',
      url: Cypress.config('baseUrl') + `/api/v1/Inscricao/${inscricaoId}/cancelar`,
      headers: {
        accept: 'application/json, text/plain, */*'
      },
      failOnStatusCode: false
    }).as('response')
  })
})

Então('retorna o status 401 de inscrição manual sem sucesso', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})
