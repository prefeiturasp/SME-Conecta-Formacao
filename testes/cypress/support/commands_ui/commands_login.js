import Login_Conecta_Localizadores from '../locators/login_locators'

const login_Conecta_Localizadores = new Login_Conecta_Localizadores()

Cypress.Commands.add('configurar_visualizacao', (device) => {
	cy.visit(Cypress.config('baseUrl'))
	switch (device) {
		case 'web':
			cy.viewport(1920, 1080)
			break
		default:
			break
	}
})

Cypress.Commands.add('vizualicacao_login', () => { 
  cy.visit('/login')

  cy.get(login_Conecta_Localizadores.texto_usuario(), { timeout: 4000 })
    .should('be.visible')
})

Cypress.Commands.add('realizar_login', (perfil) => {

  const perfilFormatado = perfil.toUpperCase()

	switch (perfilFormatado) {

    case "ADMIN":
      cy.get(login_Conecta_Localizadores.texto_usuario(), { timeout: 4000 })
       .should('be.visible')
       .type(Cypress.env('LOGIN_ADM_GERAL'))

      cy.get(login_Conecta_Localizadores.texto_senha(), { timeout: 4000 })
       .should('be.visible')
       .type(Cypress.env('SENHA'), { log: false })

      cy.get(login_Conecta_Localizadores.botao_acessar(), { timeout: 4000 })
       .should('be.visible')
       .click()

      cy.contains('Acompanhamento de propostas formativas', { timeout: 10000 })
       .should('be.visible')

      cy.get(login_Conecta_Localizadores.card_usuario(), { timeout: 4000 })
       .should('be.visible')
       .click()

       cy.contains('.ant-dropdown-menu-title-content', 'Admin DF', { timeout: 4000 })
        .should('be.visible')
        .click()
       break

		case "CURSISTA":
			cy.get(login_Conecta_Localizadores.texto_usuario(), { timeout: 4000 })
			  .should('be.visible')
			  .type(Cypress.env('LOGIN_CURSISTA'))

			cy.get(login_Conecta_Localizadores.texto_senha(), { timeout: 4000 })
			  .should('be.visible')
			  .type(Cypress.env('SENHA'), { log: false })

			cy.get(login_Conecta_Localizadores.botao_acessar(), { timeout: 10000 })
			  .should('be.visible')
			  .click()

			cy.contains('Minhas Inscrições', { timeout: 10000 }).should('be.visible')
			break

		default:
			throw new Error(`Perfil não encontrado: ${perfil}`)
	}
})

Cypress.Commands.add('clicar_botao_acessar', () => {
	cy.get(login_Conecta_Localizadores.botao_acessar())
	  .should('be.visible').click()	
})

Cypress.Commands.add('validar_acesso_conecta', () => {
	cy.get(login_Conecta_Localizadores.logo_conecta())
	  .should('be.visible')
})

Cypress.Commands.add('validar_campos_obrigatorios_acesso', (campo) => {

  if (campo === 'login') {    
    cy.get(login_Conecta_Localizadores.texto_senha())
      .type(Cypress.env('SENHA'))
  }

  if (campo === 'senha_admin') {    
    cy.get(login_Conecta_Localizadores.texto_usuario())
      .type(Cypress.env('LOGIN_ADM_GERAL'))
  }

   if (campo === 'senha_cursista') {    
    cy.get(login_Conecta_Localizadores.texto_usuario())
      .type(Cypress.env('LOGIN_CURSISTA'))
  }

  cy.get(login_Conecta_Localizadores.botao_acessar())
    .should('be.visible')
    .click()

  cy.get(login_Conecta_Localizadores.texto_obrigatorio())
    .should('be.visible')

  cy.contains('Você precisa informar um usuário e senha para acessar o sistema')
    .should('be.visible')
})

Cypress.Commands.add('validar_caracteres_acesso', (campo, dado) => {

  if (campo === 'login') {
    cy.get(login_Conecta_Localizadores.texto_usuario())
      .clear()
      .type(dado)

    cy.get(login_Conecta_Localizadores.texto_senha())
      .clear()
      .type(Cypress.env('SENHA'))
  }

  if (campo === 'senha') {
    cy.get(login_Conecta_Localizadores.texto_usuario())
      .clear()
      .type(Cypress.env('LOGIN_ADM_GERAL'))

    cy.get(login_Conecta_Localizadores.texto_senha())
      .clear()
      .type(dado)
  }

  cy.get(login_Conecta_Localizadores.botao_acessar())
    .should('be.visible')
    .click()

  cy.get(login_Conecta_Localizadores.texto_obrigatorio())
    .should('be.visible')

  cy.contains('Você precisa informar um usuário e senha para acessar o sistema')
    .should('be.visible')
})

Cypress.Commands.add('validar_acesso_invalido', (campo, dado) => {

  if (campo === 'login') {
    cy.get(login_Conecta_Localizadores.texto_usuario())
      .clear()
      .type(dado)

    cy.get(login_Conecta_Localizadores.texto_senha())
      .clear()
      .type(Cypress.env('SENHA'))
  }

  if (campo === 'senha') {
    cy.get(login_Conecta_Localizadores.texto_usuario())
      .clear()
      .type(Cypress.env('LOGIN_ADM_GERAL'))

    cy.get(login_Conecta_Localizadores.texto_senha())
      .clear()
      .type(dado)
  }

  cy.get(login_Conecta_Localizadores.botao_acessar())
    .should('be.visible')
    .click()

  cy.get(login_Conecta_Localizadores.texto_obrigatorio())
    .should('be.visible')

  cy.contains('Usuário ou senha inválidos')
    .should('be.visible')
})