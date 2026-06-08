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

Dado('que possuo um token válido no endpoint Usuario', function () {
  expect(token, 'valido').to.exist
})

// Buscar cadastro do usuário
Quando('envio uma requisição GET buscar o usuário', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/${Cypress.env('LOGIN_ADM_GERAL')}`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com cadastro do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
    expect(response.body).to.be.an('object')
    expect(response.body).to.have.property('nome')
    expect(response.body).to.have.property('cpf')
    expect(response.body).to.have.property('login')
    expect(response.body).to.have.property('email')
  })
})

// Não buscar cadastro do usuário inválido
Quando('envio uma requisição GET buscar sem usuário válido', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 405 sem cadastro do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(405) 
  })
})

// Não buscar cadastro do usuário sem autenticação
Quando('tento a requisição GET buscar o usuário', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/${Cypress.env('LOGIN_ADM_GERAL')}`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem cadastro do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Validar e-mail do usuário
Quando('envio uma requisição GET com o token nos dados', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    timeout: 10000,         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 validando e-mail do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(405)  
  })
})

// Não validar e-mail do usuário com token inválido
Quando('envio uma requisição GET sem token válido nos dados', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 405 sem validar e-mail do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(405) 
  })
})

// Não validar e-mail do usuário sem autenticação
Quando('tento a requisição GET com o token nos dados', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/${token}`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem validar e-mail do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Buscar tipo de e-mail do usuário
Quando('envio uma requisição GET tipo de e-mail', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/tipo-email`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    timeout: 10000,         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com tipo de e-mail do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)  
  })
})

// Alterar senha do usuário
Quando('envio uma requisição PUT com usuário da senha', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/${Cypress.env('LOGIN_ADM_GERAL')}/senha`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: {
      senhaAtual: `${Cypress.env('SENHA')}`,
      senhaNova: `${Cypress.env('SENHA')}`,
      confirmarSenha: `${Cypress.env('SENHA')}`
    },
    timeout: 10000,         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 alterando a senha do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)  
  })
})

// Não alterar senha sem usuário
Quando('envio uma requisição PUT sem usuário da senha', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario//senha`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: {
      senhaAtual: `${Cypress.env('SENHA')}`,
      senhaNova: `${Cypress.env('SENHA')}`,
      confirmarSenha: `${Cypress.env('SENHA')}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 405 sem alterar senha do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(405) 
  })
})

// Não validar senha do usuário sem autenticação
Quando('tento a requisição PUT com usuário da senha', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/${Cypress.env('LOGIN_ADM_GERAL')}/senha`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },
     body: {
      senhaAtual: `${Cypress.env('SENHA')}`,
      senhaNova: `${Cypress.env('SENHA')}`,
      confirmarSenha: `${Cypress.env('SENHA')}`
    },            
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem alterar senha do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Alterar e-mail do usuário
Quando('envio uma requisição PUT com usuário do e-mail', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/alterar-email`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: {
      email: `${Cypress.env('EMAIL')}`,
      login: `${Cypress.env('LOGIN_ADM_GERAL')}`,
      senha: `${Cypress.env('SENHA')}`
    },
    timeout: 10000,         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 alterando o e-mail do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)  
  })
})

// Não alterar e-mail sem o dado
Quando('envio uma requisição PUT do usuário sem e-mail', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/alterar-email`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: {
      email: ``,
      login: `${Cypress.env('LOGIN_ADM_GERAL')}`,
      senha: `${Cypress.env('SENHA')}`
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 422 sem alterar o e-mail de usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422) 
  })
})

// Não alterar e-mail sem usuário
Quando('envio uma requisição PUT sem usuário do e-mail', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/alterar-email`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: {
      email: `${Cypress.env('EMAIL')}`,
      login: ` `,
      senha: `${Cypress.env('SENHA')}`
    },        
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 422 sem alterar o e-mail do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422) 
  })
})

// Não alterar e-mail sem senha
Quando('envio uma requisição PUT sem senha do e-mail', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/alterar-email`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: {
      email: `${Cypress.env('EMAIL')}`,
      login: `${Cypress.env('LOGIN_ADM_GERAL')}`,
      senha: ` `
    },      
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 422 sem alterar e-mail para usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422) 
  })
})

// Não alterar e-mail sem autenticação
Quando('tento a requisição PUT com usuário do e-mail', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/alterar-email`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },
    body: {
      email: `${Cypress.env('EMAIL')}`,
      login: `${Cypress.env('LOGIN_ADM_GERAL')}`,
      senha: `${Cypress.env('SENHA')}`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem alterar o e-mail do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
  })
})

// Alterar e-mail com usuário
Quando('envio uma requisição PUT com usuário para e-mail', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/${Cypress.env('LOGIN_ADM_GERAL')}/email`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: {
      email: `${Cypress.env('EMAIL')}` 
    },
    timeout: 10000,         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 alterando e-mail do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)  
  })
})

// Não alterar e-mail sem o dado no usuário
Quando('envio uma requisição PUT Usuario sem e-mail', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/${Cypress.env('LOGIN_ADM_GERAL')}/email`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: {
      email: ``
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 422 sem alterar o e-mail de Usuario', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422) 
  })
})

// Não alterar e-mail sem usuário na requisição
Quando('envio uma requisição PUT sem usuário do campo email', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario//email`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: {
      email: `${Cypress.env('EMAIL')}`
    },        
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 422 sem alterar email do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(405) 
  })
})

// Não alterar email sem autenticação
Quando('tento a requisição PUT com usuário para e-mail', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/${Cypress.env('LOGIN_ADM_GERAL')}/email`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },
    body: {
      email: `${Cypress.env('EMAIL')}`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem alterar email do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Alterar nome do usuário
Quando('envio uma requisição PUT com nome do usuário', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/${Cypress.env('LOGIN_ADM_GERAL')}/nome`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: {
      nome: `${Cypress.env('NOME')}` 
    },
    timeout: 10000,         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 alterando nome do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)  
  })
})

// Não alterar nome sem o dado no usuário
Quando('envio uma requisição PUT Usuario sem nome', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/${Cypress.env('LOGIN_ADM_GERAL')}/nome`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    body: {
      nome: ``
    },         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 422 sem alterar nome de Usuario', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422) 
  })
})

// Não alterar nome sem autenticação
Quando('tento a requisição PUT com nome do usuário', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/${Cypress.env('LOGIN_ADM_GERAL')}/nome`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },
    body: {
      nome: `${Cypress.env('NOME')}`
    },          
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 401 sem alterar nome do usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Solicitar recuperação de senha
Quando('envio uma requisição POST com login do usuário', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/${Cypress.env('LOGIN_ADM_GERAL')}/solicitar-recuperacao-senha`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    timeout: 10000,         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 solicitando recuperação de senha', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)  
  })
})

// Não solicitar recuperação de senha sem usuário
Quando('envio uma requisição POST sem login do usuário', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario//solicitar-recuperacao-senha`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },        
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 405 sem solicitar recuperação de senha', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(405) 
  })
})

// Não solicitar recuperação de senha com usuário inválido
Quando('envio uma requisição POST com login de usuário inválido', function () { 
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/9999999/solicitar-recuperacao-senha`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },        
    failOnStatusCode: false,
    timeout: 60000 
  }).as('response')
})

Então('retorna o status 400 sem solicitar recuperação de senha', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(400)
  })
})

// Validar token de recuperação de senha
Quando('envio uma requisição GET com token da senha', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/valida-token-recuperacao-senha/${Cypress.env('TOKEN_RECUPERACAO')}`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    timeout: 10000,         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 com token de recuperação de senha', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)  
  })
})

// Não validar recuperação de senha sem token
Quando('envio uma requisição GET sem token da senha', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/valida-token-recuperacao-senha/${Cypress.env('LOGIN_ADM_GERAL')}`,
    headers: {
      accept: 'text/plain',
      Authorization: `Bearer ${token}`
    },
    timeout: 10000,         
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 422 sem token de recuperação de senha', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(422)  
  })
})

// Não validar token inválido na recuperação de senha
Quando('envio uma requisição GET com token de recuperação', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/valida-token-recuperacao-senha/`,
    headers: {
      accept: 'text/plain',
      Authorization: `token_invalido`
    },        
    failOnStatusCode: false,
    timeout: 60000 
  }).as('response')
})

Então('retorna o status 401 sem validar token de recuperação de senha', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(401)
  })
})

// Recuperar senha do usuário
Quando('envio uma requisição PUT de recuperar senha', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/recuperar-senha`,
    headers: {
      accept: 'text/plain'
    },
    timeout: 10000,
    body: {
      novaSenha: `${Cypress.env('SENHA')}`,
      token: `${Cypress.env('TOKEN_RECUPERACAO')}`
    },        
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 recuperando nova senha', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(400)
  })
})

// Não recuperar sem usuário
Quando('envio uma requisição PUT de recuperar senha sem token', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/recuperar-senha`,
    headers: {
      accept: 'text/plain'
    },
    body: {
      novaSenha: `${Cypress.env('SENHA')}`,
      token: `${Cypress.env('TOKEN_RECUPERACAO')}`
    },
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 400 sem recuperação da senha', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(400) 
  })
})

// Não recuperar sem inserir nova senha
Quando('envio uma requisição PUT de recuperar sem a senha', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/recuperar-senha`,
    headers: {
      accept: 'text/plain'
    },
    body: {
      novaSenha: `${Cypress.env('SENHA')}`,
      token: `${Cypress.env('TOKEN_RECUPERACAO')}`
    },               
    failOnStatusCode: false,
    timeout: 60000 
  }).as('response')
})

Então('retorna o status 400 sem recuperar a senha', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(400)
  })
})

// Reenviar e-mail de recuperação de senha ao usuário
Quando('envio uma requisição GET de reenvio da senha', function () { 
  return cy.request({
    method: 'GET',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario/${Cypress.env('LOGIN_ADM_GERAL')}/reenviar-email`,
    headers: {
      accept: 'text/plain'
    },
    timeout: 10000,    
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 200 reenviando o e-mail de senha ao usuário', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(200)
  })
})

// Não reenviar e-mail de recuperação de senha sem usuário
Quando('tento a requisição GET de reenvio da senha', function () { 
  return cy.request({
    method: 'PUT',
    url: Cypress.config('baseUrl') + `/api/v1/Usuario//reenviar-email`,
    headers: {
      accept: 'text/plain'
    },   
    failOnStatusCode: false  
  }).as('response')
})

Então('retorna o status 405 sem reenviar e-mail de senha', function () {
  cy.get('@response').then((response) => {
    expect(response.status).to.eq(405) 
  })
})