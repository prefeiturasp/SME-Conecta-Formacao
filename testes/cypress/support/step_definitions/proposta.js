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
  expect(Boolean(token), 'token de acesso foi informado').to.be.true
})

// Criar uma nova proposta
Quando('envio uma requisição POST em Proposta', function () {
  cy.request({
    method: 'POST',
    url: `${Cypress.config('baseUrl')}/api/v1/Proposta`,
    headers: {
      accept: 'text/plain',   
      Authorization: `Bearer ${token}`
    },
    body: {
      ehProximoPasso: false,
      situacao: 2,
      formacaoHomologada: 1,
      tipoFormacao: 1,
      formato: 2,
      dreIdPropostas: null,
      nomeFormacao: 'Teste automatizado',
      quantidadeTurmas: '1',
      quantidadeVagasTurma: '1',
      funcaoEspecificaOutros: '',
      publicoAlvoOutros: '',
      criterioValidacaoInscricaoOutros: '',
      sobreEsteCurso: '',
      outrosCriterios: '',
      cursoComCertificado: false,
      tipoEmissor: null,
      idEmissor: null,
      acaoInformativa: false,
      acaoFormativaTexto: '',
      acaoFormativaLink: '',
      integrarNoSGA: true,
      desativarAnoEhComponente: false,
      revalidacao: false,
      numeroHomologacao: null,
      tiposInscricao: [
        { tipoInscricao: 4 },
        { tipoInscricao: 2 }
      ],
      publicosAlvo: [
        { cargoFuncaoId: 1 }
      ],
      funcoesEspecificas: [],
      vagasRemanecentes: [],
      criteriosValidacaoInscricao: [
        { criterioValidacaoInscricaoId: 1 }
      ],
      palavrasChaves: [],
      criterioCertificacao: [],
      dres: [
        { dreId: 3 }
      ],
      modalidades: [],
      anosTurmas: [],
      componentesCurriculares: [],
      turmas: [
        {
          nome: 'Turma 1',
          dresIds: [3]
        }
      ],
      gruposPeriodos: [],
      pareceristas: []
    },
    failOnStatusCode: false
  }).as('response')
})

Então('retorna o status 200 criando uma nova proposta', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.equal(200)
    expect(response.body).to.have.property('entidadeId')
    expect(response.body.entidadeId).to.exist

    Cypress.env('ENTIDADE_ID', response.body.entidadeId)
  })
})

// Não criar proposta sem dados obrigatórios
Quando('envio uma requisição POST em Proposta sem dados', function () {
  cy.request({
    method: 'POST',
    url: `${Cypress.config('baseUrl')}/api/v1/Proposta`,
    headers: {
      accept: 'text/plain',   
      Authorization: `Bearer ${token}`
    },
    body: {
      ehProximoPasso: false,  
      dreIdPropostas: null,       
      funcaoEspecificaOutros: '',
      publicoAlvoOutros: '',
      criterioValidacaoInscricaoOutros: '',
      sobreEsteCurso: '',
      outrosCriterios: '',
      cursoComCertificado: false,
      tipoEmissor: null,
      idEmissor: null,
      acaoInformativa: false,
      acaoFormativaTexto: '',
      acaoFormativaLink: '',
      integrarNoSGA: true,
      desativarAnoEhComponente: false,
      revalidacao: false,
      numeroHomologacao: null,     
      funcoesEspecificas: [],
      vagasRemanecentes: [],     
      palavrasChaves: [],
      criterioCertificacao: [],  
      modalidades: [],
      anosTurmas: [],
      componentesCurriculares: [],      
      gruposPeriodos: [],
      pareceristas: []
    },
    failOnStatusCode: false
  }).as('response')
})

Então('retorna o status 400 sem criar proposta', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.equal(400)
  })
})

// Não criar proposta sem autenticação
Quando('tento uma requisição POST em Proposta', function () {
  cy.request({
    method: 'POST',
    url: `${Cypress.config('baseUrl')}/api/v1/Proposta`,
    headers: {
      accept: 'text/plain',   
      Authorization: `token_invalido`
    },
    body: {
      ehProximoPasso: false,
      situacao: 2,
      formacaoHomologada: 1,
      tipoFormacao: 1,
      formato: 2,
      dreIdPropostas: null,
      nomeFormacao: 'Teste automatizado',
      quantidadeTurmas: '1',
      quantidadeVagasTurma: '1',
      funcaoEspecificaOutros: '',
      publicoAlvoOutros: '',
      criterioValidacaoInscricaoOutros: '',
      sobreEsteCurso: '',
      outrosCriterios: '',
      cursoComCertificado: false,
      tipoEmissor: null,
      idEmissor: null,
      acaoInformativa: false,
      acaoFormativaTexto: '',
      acaoFormativaLink: '',
      integrarNoSGA: true,
      desativarAnoEhComponente: false,
      revalidacao: false,
      numeroHomologacao: null,
      tiposInscricao: [
        { tipoInscricao: 4 },
        { tipoInscricao: 2 }
      ],
      publicosAlvo: [
        { cargoFuncaoId: 1 }
      ],
      funcoesEspecificas: [],
      vagasRemanecentes: [],
      criteriosValidacaoInscricao: [
        { criterioValidacaoInscricaoId: 1 }
      ],
      palavrasChaves: [],
      criterioCertificacao: [],
      dres: [
        { dreId: 3 }
      ],
      modalidades: [],
      anosTurmas: [],
      componentesCurriculares: [],
      turmas: [
        {
          nome: 'Turma 1',
          dresIds: [3]
        }
      ],
      gruposPeriodos: [],
      pareceristas: []
    },
    failOnStatusCode: false
  }).as('response')
})

Então('retorna o status 401 sem criar proposta', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.equal(401)
  })
})

// Atualizar proposta
Quando('envio uma requisição PUT em Proposta', function () {
  const entidadeId = Cypress.env('ENTIDADE_ID')

  expect(entidadeId, 'ID da proposta').to.exist

  cy.request({
    method: 'PUT',
    url: `${Cypress.config('baseUrl')}/api/v1/Proposta/${entidadeId}`,
    headers: {
      accept: 'application/json, text/plain, */*',
      'content-type': 'application/json',
      Authorization: `Bearer ${Cypress.env('TOKEN')}`
    },
    body: {
      anexoUrl: '',
      ehProximoPasso: false,
      situacao: 2,
      formacaoHomologada: 1,
      tipoFormacao: 1,
      formato: 1,
      dreIdPropostas: null,
      nomeFormacao: 'Teste automatizado',
      quantidadeTurmas: 3,
      quantidadeVagasTurma: 30,
      funcaoEspecificaOutros: '',
      publicoAlvoOutros: '',
      criterioValidacaoInscricaoOutros: '',
      pareceristas: []
    },
    failOnStatusCode: false
  }).as('responsePut')
})

Então('retorna o status 200 atualizando a proposta', function () {
  cy.get('@responsePut').then((response) => {
    expect(response.status).to.equal(200)
    expect(response.body).to.have.property('sucesso')
    expect(response.body).to.have.property('entidadeId')
    expect(response.body).to.have.property('mensagem')
  })
})

// Não atualizar proposta sem o id
Quando('envio uma requisição PUT em Proposta sem id', function () {
  cy.request({
    method: 'PUT',
    url: `${Cypress.config('baseUrl')}/api/v1/Proposta/ `,
    headers: {
      accept: 'application/json, text/plain, */*',
      'content-type': 'application/json',
      Authorization: `Bearer ${Cypress.env('TOKEN')}`
    },
      body: {
        anexoUrl: '',
        ehProximoPasso: false,
        situacao: 2,
        formacaoHomologada: 1,
        tipoFormacao: 1,
        formato: 1,
        dreIdPropostas: null,
        nomeFormacao: 'Teste automatizado',
        quantidadeTurmas: 3,
        quantidadeVagasTurma: 30,
        funcaoEspecificaOutros: '',
        publicoAlvoOutros: '',
        criterioValidacaoInscricaoOutros: '',
        pareceristas: []
      },
    failOnStatusCode: false
  }).as('responsePut')
})

Então('retorna o status 405 sem atualizar a proposta', function () {
  cy.get('@responsePut').then((response) => {
    expect(response.status).to.equal(405)
  })
})

// Não atualizar proposta sem autenticação
Quando('tento uma requisição PUT em Proposta', function () {
  cy.request({
    method: 'PUT',
    url: `${Cypress.config('baseUrl')}/api/v1/Proposta/1`,
    headers: {
      accept: 'application/json, text/plain, */*',
      'content-type': 'application/json',
      Authorization: `token_invalido`
      },
      body: {
        anexoUrl: '',
        ehProximoPasso: false,
        situacao: 2,
        formacaoHomologada: 1,
        tipoFormacao: 1,
        formato: 1,
        dreIdPropostas: null,
        nomeFormacao: 'Teste automatizado',
        quantidadeTurmas: 3,
        quantidadeVagasTurma: 30,
        funcaoEspecificaOutros: '',
        publicoAlvoOutros: '',
        criterioValidacaoInscricaoOutros: '',
        pareceristas: []
      },
    failOnStatusCode: false
  }).as('responsePut')
})

Então('retorna o status 401 sem atualizar a proposta', function () {
  cy.get('@responsePut').then((response) => {
    expect(response.status).to.equal(401)
  })
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

// Excluir proposta
Quando('envio uma requisição DELETE em Proposta', function () {
  const entidadeId = Cypress.env('ENTIDADE_ID')

  expect(entidadeId, 'ID da proposta').to.exist

  cy.request({
    method: 'DELETE',
    url: `${Cypress.config('baseUrl')}/api/v1/Proposta/${entidadeId}`,
    headers: {
      accept: 'application/json, text/plain, */*',
      Authorization: `Bearer ${Cypress.env('TOKEN')}`
    },
    failOnStatusCode: false
  }).as('responseDelete')
})

Então('retorna o status 200 com exclusão da proposta', function () {
  cy.get('@responseDelete').then((response) => {
    expect(response.status).to.equal(200)
  })
})

// Não excluir proposta sem o id
Quando('envio uma requisição DELETE em Proposta sem id', function () {
  cy.request({
      method: 'DELETE',
      url: `${Cypress.config('baseUrl')}/api/v1/Proposta/ `,
      headers: {
        accept: 'application/json, text/plain, */*',
        Authorization: `Bearer ${Cypress.env('TOKEN')}`
      },
      failOnStatusCode: false
  }).as('responseDelete')
})

Então('retorna o status 405 sem excluir proposta', function () {
  cy.get('@responseDelete').then((response) => {
    expect(response.status).to.equal(405)
  })
})

// Não excluir proposta sem autenticação
Quando('tento uma requisição DELETE em Proposta', function () {
  cy.request({
    method: 'DELETE',
    url: `${Cypress.config('baseUrl')}/api/v1/Proposta/1`,
    headers: {
      accept: 'application/json, text/plain, */*',
      Authorization: `token_invalido`,
    },
    failOnStatusCode: false
  }).as('responseDelete')
})

Então('retorna o status 401 sem excluir a proposta', function () {
  cy.get('@responseDelete').then((response) => {
    expect(response.status).to.equal(401)
  })
})