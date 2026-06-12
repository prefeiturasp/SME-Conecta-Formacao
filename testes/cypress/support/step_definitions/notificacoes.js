import { When, Then } from '@badeball/cypress-cucumber-preprocessor'

const Quando = When
const Então = Then

Quando('acesso o menu Notificações', () => {
  cy.acessar_notificacoes()
})

Então('visualizo as novas notificações na listagem', () => {
  cy.validar_acesso_notificacoes()
})

Quando('preencho o campo {string} com {string} nas notificações', (tipo, valor) => {
  cy.preencher_campos_notificacoes(tipo, valor)
})

Então('busca na listagem em notificações com {string}', (campo) => {
  cy.validar_filtros_notificacoes(campo)
})

