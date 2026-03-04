import Login_CDEP_Localizadores from '../locators/login_locators'

const login_CDEP_Localizadores = new Login_CDEP_Localizadores

Cypress.Commands.add('login_CDEP', (device) => {
	cy.configurar_visualizacao(device)
})

Cypress.Commands.add('realizar_login', (perfil) => {
	switch (perfil) {
		case "Admin":
			cy.get(login_CDEP_Localizadores.texto_usuario())
			  .type(Cypress.config('usuario_homol_admin'))
			cy.get(login_CDEP_Localizadores.texto_senha())
			  .type(Cypress.config('senha_homol'))
			cy.get(login_CDEP_Localizadores.botao_acessar())
			  .should('be.visible').click()

			cy.url().should('include', 'indicadores')
			break

		case "Externo":
			cy.get(login_CDEP_Localizadores.texto_usuario())
			  .type(Cypress.config('usuario_homol_externo'))
			cy.get(login_CDEP_Localizadores.texto_senha())
			  .type(Cypress.config('senha_homol'))
			cy.get(login_CDEP_Localizadores.botao_acessar())
			  .should('be.visible').click()

			cy.contains('Minhas solicitações').should('be.visible')
			break

		default:
			console.error("Perfil não encontrado!")
	}
})