Cypress.Commands.add('autenticar_login', (usuario, senha) => {
  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + '/api/v1/autenticacao',
    body: { login: usuario, senha: senha },
    timeout: 60000,
    failOnStatusCode: false,
  }).then((response) => {
    if (response.status === 200) {
      globalThis.token = response.body.token
    }
    return response
  })
})

Cypress.Commands.add('gerar_token', () => {
  const tokenExistente = Cypress.env('TOKEN')

  if (tokenExistente) {
    return cy.wrap(tokenExistente)
  }

  return cy.request({
    method: 'POST',
    url: Cypress.config('baseUrl') + '/api/v1/autenticacao',
    body: {
      login: Cypress.env('LOGIN_ADM_GERAL'),
      senha: Cypress.env('SENHA'),
    },
    headers: {
      'Content-Type': 'application/json'
    },
    timeout: 60000,
    failOnStatusCode: false,
  }).then((response) => {

    expect(response.status).to.eq(200)

    const perfis = response.body.perfilUsuario

    const perfilAdminDF = perfis.find(p => p.perfilNome === 'Admin DF')

    expect(perfilAdminDF, 'Perfil Admin DF não encontrado').to.exist

    return cy.request({
      method: 'PUT',
      url: `${Cypress.config('baseUrl')}/api/v1/autenticacao/perfis/${perfilAdminDF.perfil}`,
      headers: {
        Authorization: `Bearer ${response.body.token}`,
        'Content-Type': 'application/json'
      },
      failOnStatusCode: false
    }).then((resPerfil) => {

      expect(resPerfil.status).to.eq(200)

      const tokenFinal = resPerfil.body.token

      Cypress.env('TOKEN', tokenFinal)

      return tokenFinal
    })
  })
})