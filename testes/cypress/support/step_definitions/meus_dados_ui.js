import { When, Then } from '@badeball/cypress-cucumber-preprocessor'

const Quando = When
const Então = Then

Quando('acesso o menu Meus Dados', () => {
  cy.acessar_menu_meus_dados()
})

Então('os campos de Meus Dados devem estar preenchidos para {string}', (campo) => {
  cy.validar_campo_meus_dados(campo)
})

Quando('clico em alterar {string} nos meus dados', (campo) => {
  cy.clicar_alterar(campo)
})

Quando('clico em cancelar no modal de alteração', () => {
  cy.clicar_modal_cancelar()
})

Quando('clico em salvar no modal de alteração de dados', () => {
  cy.clicar_salvar_modal()
})

Então('o modal de alteração deve ser exibido', () => {
  cy.validar_modal_alteracao_visivel()
})

Então('o modal de alteração não deve estar visível', () => {
  cy.validar_modal_alteracao_nao_visivel()
})

Então('o campo {string} do modal de senha deve estar visível', (campo) => {
  cy.validar_campos_modal_senha(campo)
})

Quando('preencho o modal de senha com dados válidos', () => {
  cy.preencher_modal_senha()
})

Então('realiza a alteração de senha', () => {
  cy.validar_alteracao_meus_dados()
})

Então('mensagem de alteração dos meus dados deve ser exibida', () => {
  cy.validar_alteracao_meus_dados()
})