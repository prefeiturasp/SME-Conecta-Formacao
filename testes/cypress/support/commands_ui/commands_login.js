import Login_Conecta_Localizadores from '../locators/login_locators'

const login_Conecta_Localizadores = new Login_Conecta_Localizadores()

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