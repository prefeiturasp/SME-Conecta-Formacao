import Redefinir_Senha_Conecta_Localizadores from '../locators/redefinir_senha_locators'

const redefinir_senha_Conecta_Localizadores = new Redefinir_Senha_Conecta_Localizadores()

Cypress.Commands.add('clicar_botao_esqueci_senha', () => {
	cy.get(redefinir_senha_Conecta_Localizadores.botao_esqueci_senha())
	  .should('be.visible').click()	
})

Cypress.Commands.add('clicar_botao_continuar_esqueci_senha', () => {
  cy.get(redefinir_senha_Conecta_Localizadores.input_usuario_esqueci_senha())
    .should('be.visible').type(Cypress.env('LOGIN_ADM_GERAL'))

	cy.get(redefinir_senha_Conecta_Localizadores.botao_continuar_esqueci_senha())
	  .should('be.visible').click()	
})

Cypress.Commands.add('validar_enviado_esqueci_senha', () => {
  cy.get(redefinir_senha_Conecta_Localizadores.modal_email_enviado())
    .should('be.visible')
    .should('contain.text', 'As orientações para recuperação de senha foram enviados para')
})

Cypress.Commands.add('clicar_botao_continuar_esqueci_senha_invalido', () => {
  cy.get(redefinir_senha_Conecta_Localizadores.input_usuario_esqueci_senha())
    .should('be.visible').type('123456')

	cy.get(redefinir_senha_Conecta_Localizadores.botao_continuar_esqueci_senha())
	  .should('be.visible').click()	
})

Cypress.Commands.add('validar_enviado_esqueci_senha_invalido', () => {
  cy.get(redefinir_senha_Conecta_Localizadores.alerta_usuario_invalido(), { timeout: 60000 })
    .should('be.visible')
    .should('contain.text', 'Usuário não encontrado.')
})

Cypress.Commands.add('clicar_botao_continuar_esqueci_senha_caracteres', () => {
  cy.get(redefinir_senha_Conecta_Localizadores.input_usuario_esqueci_senha())
    .should('be.visible').type('1234')

	cy.get(redefinir_senha_Conecta_Localizadores.botao_continuar_esqueci_senha())
	  .should('be.visible').click()	
})

Cypress.Commands.add('validar_enviado_esqueci_senha_caracteres', () => {
  cy.get(redefinir_senha_Conecta_Localizadores.alerta_caracteres(), { timeout: 60000 })
    .should('be.visible')
    .should('contain.text', 'Deve conter no mínimo 5 caracteres')
})

Cypress.Commands.add('acessar_redefinir_senha', () => { 
  cy.visit(`/redefinir-senha/${Cypress.env('TOKEN_RECUPERACAO')}`)

  cy.url().should('contain', '/redefinir-senha')
})

Cypress.Commands.add('validar_acesso_link_expirado', () => {
  cy.contains(
    'Este link expirou, utilize a opção "Esqueci minha senha" para solicitar um novo link.',
    { timeout: 60000 }
  ).should('be.visible')
})