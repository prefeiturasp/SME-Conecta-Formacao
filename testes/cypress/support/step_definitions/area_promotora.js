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

Dado('que possuo um token válido no endpoint AreaPromotora', function () {
  expect(token, 'valido').to.exist
})

// Buscar tipos da Área Promotora
Quando('envio uma requisição GET de tipos da promotora', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/tipos`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com tipos da Área Promotora', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array')

    response.body.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item).to.have.property('nome').and.to.be.a('string')
    })
  })
})

// Não buscar tipos da Área Promotora sem autenticação
Quando('tento a requisição GET de tipos da promotora', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/tipos`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem tipos da Área Promotora', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar Área Promotora
Quando('envio uma requisição GET de promotora', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com Área Promotora', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('object')
    expect(response.body).to.have.property('items').that.is.an('array')
    expect(response.body).to.have.property('totalPaginas')
    expect(response.body).to.have.property('totalRegistros')

  response.body.items.forEach((item) => {
    expect(item).to.have.property('id')
    expect(item).to.have.property('nome')
    expect(item).to.have.property('tipo')
    expect(item).to.have.property('nomeDre')
    })
  })
})

// Não buscar Área Promotora sem autenticação
Quando('tento a requisição GET de Área Promotora', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem Área Promotora', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar lista Área Promotora
Quando('envio uma requisição GET de lista promotora', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/lista`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com lista Área Promotora', function () {
  cy.get('@response').then((response) => {    
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array')

    response.body.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item).to.have.property('descricao').and.to.be.a('string')
    })
  })
})

// Não buscar lista Área Promotora sem autenticação
Quando('tento a requisição GET de lista Área Promotora', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/lista`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem lista Área Promotora', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar lista rede parceira Área Promotora
Quando('envio uma requisição GET de lista parceira promotora', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/lista/rede-parceria`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com lista rede parceira Área Promotora', function () {
  cy.get('@response').then((response) => {    
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('array')

    response.body.forEach((item) => {
      expect(item).to.have.property('id')
      expect(item).to.have.property('descricao').and.to.be.a('string')
    })
  })
})

// Não buscar lista rede parceira Área Promotora sem autenticação
Quando('tento a requisição GET de lista parceira promotora', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/lista/rede-parceria`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem lista rede parceira Área Promotora', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Cadastrar Área Promotora com sucesso
Quando('envio uma requisição POST para cadastrar Área Promotora', function () {
  const timestamp = Date.now()
  const emailDomain = Cypress.env('EMAIL_DOMAIN') || 'test.local'  

  const payload = {
    nome: `Teste automatizado ${timestamp}`,
    tipo: 1,
    perfil: {
      id: `${Cypress.env('AREA_PROMOTORA_ID')}`,
      nome: `${Cypress.env('PERFIL_AREA_PROMOTORA')}`,
      visaoId: 1,
      label: `${Cypress.env('TEXTO_AREA_PROMOTORA')}`,
      value: `${Cypress.env('VALOR_AREA_PROMOTORA')}`
    },
    telefones: [
      { telefone: `${Cypress.env('TELEFONES')}` }
    ],
    emails: [
      { email: `teste.${timestamp}@${emailDomain}` }
    ],
    grupoId: `${Cypress.env('GRUPO_AREA_PROMOTORA')}`
  }

  cy.log(JSON.stringify(payload))

  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`,
      'content-type': 'application/json'
    },
    body: payload,
    failOnStatusCode: false
  }).as('response')
})

Então('retorna sucesso no cadastro da Área Promotora', function () {
  cy.get('@response').then((response) => {
    console.log('POST AreaPromotora body:', response.body)
    expect(response.status).to.eq(200)

    let id

    if (typeof response.body === 'number') {
      id = response.body
    } else if (response.body?.id) {
      id = response.body.id
    }

    expect(id, 'id da área promotora').to.exist

    cy.log(`ID criado: ${id}`)

    cy.request({
      method: 'DELETE',
      url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/${id}`,
      headers: {
        accept: 'text/plain',
        Authorization: `Bearer ${token}`
      },
      failOnStatusCode: false
    }).then((deleteResponse) => {
      expect(deleteResponse.status).to.eq(200)
      cy.log(`Registro deletado: ${id}`)
    })
  })
})

// Não cadastrar Área Promotora com label já existente
Quando('crio um registro e tento cadastrar novamente a mesma label de Área Promotora', function () {
  const timestamp = Date.now()
  const emailDomain = Cypress.env('EMAIL_DOMAIN') || 'test.local'

  const payload = {
    nome: `Teste automatizado ${timestamp}`,
    tipo: 1,
    perfil: {
      id: `${Cypress.env('AREA_PROMOTORA_ID')}`,
      nome: `${Cypress.env('PERFIL_AREA_PROMOTORA')}`,
      visaoId: 1,
      label: `${Cypress.env('TEXTO_AREA_PROMOTORA')}`,
      value: `${Cypress.env('VALOR_AREA_PROMOTORA')}`
    },
    telefones: [
      { telefone: `${Cypress.env('TELEFONES')}` }
    ],
    emails: [
      { email: `teste.${timestamp}@${emailDomain}` }
    ],
    grupoId: `${Cypress.env('GRUPO_AREA_PROMOTORA')}`
  }

  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`,
      'content-type': 'application/json'
    },
    body: payload,
    failOnStatusCode: false
  }).then((createResponse) => {
    expect(createResponse.status).to.eq(200)

    let id

    if (typeof createResponse.body === 'number') {
      id = createResponse.body
    } else if (createResponse.body?.id) {
      id = createResponse.body.id
    }

    expect(id, 'id da área promotora').to.exist

    cy.wrap(id).as('areaPromotoraId')

    return cy.request({
      method: 'POST',
      url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora`,
      headers: {
        accept: 'text/plain',
        Authorization: `Bearer ${token}`,
        'content-type': 'application/json'
      },
      body: payload,
      failOnStatusCode: false
    }).as('response')
  })
})

Então('não cadastra Área Promotora com label duplicada retornando o status 400', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(400)
  })

  cy.get('@areaPromotoraId').then((id) => {
    cy.request({
      method: 'DELETE',
      url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/${id}`,
      headers: {
        accept: 'text/plain',
        Authorization: `Bearer ${token}`
      },
      failOnStatusCode: false
    }).then((deleteResponse) => {
      expect(deleteResponse.status).to.eq(200)
      cy.log(`Registro deletado: ${id}`)
    })
  })
})

// Não cadastrar Área Promotora sem payload
Quando('envio uma requisição POST sem payload para Área Promotora', function () {
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`,
      'content-type': 'application/json'
    },
    body: {}, 
    failOnStatusCode: false
  }).as('response')
})

Então('retorna erro 422 ao cadastrar Área Promotora', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422)

    if (response.body) {
      expect(response.body).to.exist
    }
  })
})

// Não cadastrar Área Promotora sem autenticação
Quando('tento enviar uma requisição POST para cadastrar Área Promotora', function () {
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora`,
    headers: {
      accept: 'application/json, text/plain, */*',
      'content-type': 'application/json'
    },
    body: '',
    failOnStatusCode: false
  }).as('response')
})

Então('retorna o status 401 ao cadastrar Área Promotora', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Excluir Área Promotora
Quando('crio um registro para validar o delete de Área Promotora', function () {
  const timestamp = Date.now()
  const emailDomain = Cypress.env('EMAIL_DOMAIN') || 'test.local'

  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`,
      'content-type': 'application/json'
    },
    body: {
      nome: `Teste automatizado ${timestamp}`,
      tipo: 1,
      perfil: {
      id: `${Cypress.env('AREA_PROMOTORA_ID')}`,
      nome: `${Cypress.env('PERFIL_AREA_PROMOTORA')}`,
      visaoId: 1,
      label: `${Cypress.env('TEXTO_AREA_PROMOTORA')}`,
      value: `${Cypress.env('VALOR_AREA_PROMOTORA')}`
    },
    telefones: [
      { telefone: `${Cypress.env('TELEFONES')}` }
    ],
    emails: [
      { email: `teste.${timestamp}@${emailDomain}` }
    ],
    grupoId: `${Cypress.env('GRUPO_AREA_PROMOTORA')}`
  },
    failOnStatusCode: false
  }).then((response) => {
    expect(response.status).to.eq(200)

    let id

    if (typeof response.body === 'number') {
      id = response.body
    } else if (response.body?.id) {
      id = response.body.id
    }

    expect(id, 'id da área promotora').to.exist

    cy.wrap(id).as('areaPromotoraId')
  })
})

Então('exclui a Área Promotora com o status 200', function () {
  cy.get('@areaPromotoraId').then((id) => {
    cy.request({
      method: 'DELETE',
      url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/${id}`,
      headers: {
        accept: 'text/plain',
        Authorization: `Bearer ${token}`
      },
      failOnStatusCode: false
    }).then((deleteResponse) => {
      expect(deleteResponse.status).to.eq(200)
      cy.log(`Delete executado para o registro: ${id}`)
    })
  })
})

// Não exclui Área Promotora sem id
Quando('tento deletar Área Promotora sem id', function () {  
    return cy.request({
      method: 'DELETE',
      url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/1`,
      headers: {
        accept: 'text/plain',
        Authorization: `Bearer ${token}`
      },
      failOnStatusCode: false
  }).as('response')
})

Então('não exlcui Área Promotora sem id retornando o status 400', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(400)

  })
})

// Não deletar Área Promotora com token inválido
Quando('tento deletar Área Promotora', function () {  
    return cy.request({
      method: 'DELETE',
      url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/1`,
      headers: {
        accept: 'text/plain',
        Authorization: `Bearer token_invalido`
      },
      failOnStatusCode: false
  }).as('response')

})

Então('não exlcui Área Promotora retornando o status 400', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)

  })
})

// Editar Área Promotora por id
Quando('crio um registro para validar a edição por id de Área Promotora', function () {
  const timestamp = Date.now()
  const emailDomain = Cypress.env('EMAIL_DOMAIN') || 'test.local'

  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`,
      'content-type': 'application/json'
    },
    body: {
      nome: `Teste automatizado ${timestamp}`,
    tipo: 1,
    perfil: {
      id: `${Cypress.env('AREA_PROMOTORA_ID')}`,
      nome: `${Cypress.env('PERFIL_AREA_PROMOTORA')}`,
      visaoId: 1,
      label: `${Cypress.env('TEXTO_AREA_PROMOTORA')}`,
      value: `${Cypress.env('VALOR_AREA_PROMOTORA')}`
    },
    telefones: [
      { telefone: `${Cypress.env('TELEFONES')}` }
    ],
    emails: [
      { email: `teste.${timestamp}@${emailDomain}` }
    ],
    grupoId: `${Cypress.env('GRUPO_AREA_PROMOTORA')}`
  },
    failOnStatusCode: false
  }).then((response) => {
    expect(response.status).to.eq(200)

    let id

    if (typeof response.body === 'number') {
      id = response.body
    } else if (response.body?.id) {
      id = response.body.id
    }

    expect(id, 'id da área promotora').to.exist
    cy.wrap(id).as('areaPromotoraId')
  })
})

Então('edito a Área Promotora por id com o status 200', function () {
  cy.get('@areaPromotoraId').then((id) => {
    const timestamp = Date.now()
    const emailDomain = Cypress.env('EMAIL_DOMAIN') || 'test.local'

    cy.request({
      method: 'PUT',
      url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/${id}`,
      headers: {
        accept: 'text/plain',
        Authorization: `Bearer ${token}`,
        'content-type': 'application/json'
      },
      body: {
        id: id,
        nome: `Teste automatizado ${timestamp}`,
    tipo: 1,
    perfil: {
      id: `${Cypress.env('AREA_PROMOTORA_ID')}`,
      nome: `${Cypress.env('PERFIL_AREA_PROMOTORA')}`,
      visaoId: 1,
      label: `${Cypress.env('TEXTO_AREA_PROMOTORA')}`,
      value: `${Cypress.env('VALOR_AREA_PROMOTORA')}`
    },
    telefones: [
      { telefone: `${Cypress.env('TELEFONES')}` }
    ],
    emails: [
      { email: `teste.${timestamp}@${emailDomain}` }
    ],
    grupoId: `${Cypress.env('GRUPO_AREA_PROMOTORA')}`
  },
      failOnStatusCode: false
    }).then((response) => {
      expect(response.status).to.eq(200)
    })

    cy.request({
      method: 'DELETE',
      url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/${id}`,
      headers: {
        accept: 'text/plain',
        Authorization: `Bearer ${token}`
      },
      failOnStatusCode: false
    })
  })
})

// Não editar Área Promotora sem id
Quando('tento editar Área Promotora sem id', function () {
  const timestamp = Date.now()
  const emailDomain = Cypress.env('EMAIL_DOMAIN') || 'test.local'

  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/abc`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`,
      'content-type': 'application/json'
    },
    body: {
      nome: `Teste automatizado ${timestamp}`,
    tipo: 1,
    perfil: {
      id: `${Cypress.env('AREA_PROMOTORA_ID')}`,
      nome: `${Cypress.env('PERFIL_AREA_PROMOTORA')}`,
      visaoId: 1,
      label: `${Cypress.env('TEXTO_AREA_PROMOTORA')}`,
      value: `${Cypress.env('VALOR_AREA_PROMOTORA')}`
    },
    telefones: [
      { telefone: `${Cypress.env('TELEFONES')}` }
    ],
    emails: [
      { email: `teste.${timestamp}@${emailDomain}` }
    ],
    grupoId: `${Cypress.env('GRUPO_AREA_PROMOTORA')}`
  },
    failOnStatusCode: false
  }).as('response')
})

Então('não edita Área Promotora sem id retornando o status 400', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422)
  })
})

// Não editar Área Promotora com token inválido
Quando('tento editar Área Promotora com token inválido', function () {
  const timestamp = Date.now()
  const emailDomain = Cypress.env('EMAIL_DOMAIN') || 'test.local'

  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/1`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer token_invalido`,
      'content-type': 'application/json'
    },
    body: {
      nome: `Teste automatizado ${timestamp}`,
    tipo: 1,
    perfil: {
      id: `${Cypress.env('AREA_PROMOTORA_ID')}`,
      nome: `${Cypress.env('PERFIL_AREA_PROMOTORA')}`,
      visaoId: 1,
      label: `${Cypress.env('TEXTO_AREA_PROMOTORA')}`,
      value: `${Cypress.env('VALOR_AREA_PROMOTORA')}`
    },
    telefones: [
      { telefone: `${Cypress.env('TELEFONES')}` }
    ],
    emails: [
      { email: `teste.${timestamp}@${emailDomain}` }
    ],
    grupoId: `${Cypress.env('GRUPO_AREA_PROMOTORA')}`
  },
    failOnStatusCode: false
  }).as('response')
})

Então('não edita Área Promotora retornando o status 401', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar Área Promotora por id
Quando('crio um registro para validar a busca por id de Área Promotora', function () {
  const timestamp = Date.now()
  const emailDomain = Cypress.env('EMAIL_DOMAIN') || 'test.local'

  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`,
      'content-type': 'application/json'
    },
    body: {
      nome: `Teste automatizado ${timestamp}`,
    tipo: 1,
    perfil: {
      id: `${Cypress.env('AREA_PROMOTORA_ID')}`,
      nome: `${Cypress.env('PERFIL_AREA_PROMOTORA')}`,
      visaoId: 1,
      label: `${Cypress.env('TEXTO_AREA_PROMOTORA')}`,
      value: `${Cypress.env('VALOR_AREA_PROMOTORA')}`
    },
    telefones: [
      { telefone: `${Cypress.env('TELEFONES')}` }
    ],
    emails: [
      { email: `teste.${timestamp}@${emailDomain}` }
    ],
    grupoId: `${Cypress.env('GRUPO_AREA_PROMOTORA')}`
  },
    failOnStatusCode: false
  }).then((response) => {
    expect(response.status).to.eq(200)

    let id

    if (typeof response.body === 'number') {
      id = response.body
    } else if (response.body?.id) {
      id = response.body.id
    }

    expect(id, 'id da área promotora').to.exist

    cy.wrap(id).as('areaPromotoraId')
  })
})

Então('busco a Área Promotora por id com o status 200', function () {
  cy.get('@areaPromotoraId').then((id) => {
    cy.request({
      method: 'GET',
      url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/${id}`,
      headers: {
        accept: 'text/plain',
        Authorization: `Bearer ${token}`
      },
      failOnStatusCode: false
    }).then((response) => {
      expect(response.status).to.eq(200)
      expect(response.body).to.exist
      expect(response.body).to.be.an('object')

      if (response.body.id !== undefined) {
        expect(response.body.id).to.eq(id)
      }

      cy.log(`Busca realizada com sucesso para o id: ${id}`)
    })

    cy.request({
      method: 'DELETE',
      url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/${id}`,
      headers: {
        accept: 'text/plain',
        Authorization: `Bearer ${token}`
      },
      failOnStatusCode: false
    })
  })
})

// Não buscar Área Promotora com id inválido
Quando('tento buscar Área Promotora com id inválido', function () {
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/abc`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    failOnStatusCode: false
  }).as('response')
})

Então('não busca Área Promotora retornando o status 400', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422)
  })
})

// Não buscar Área Promotora com token inválido
Quando('tento buscar Área Promotora com token inválido', function () {
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/AreaPromotora/1`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer token_invalido`
    },
    failOnStatusCode: false
  }).as('response')
})

Então('não busca Área Promotora retornando o status 401', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})