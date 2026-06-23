import { When, Then } from '@badeball/cypress-cucumber-preprocessor'

const Quando = When
const Então = Then

Quando('acesso o menu Meus Certificados', () => {
  cy.acessar_meus_certificados()
})

Quando('filtro certificados obtidos nas formações', () => {
  cy.filtrar_meus_certificados()
})

Então('o sistema permite baixar certificado de conclusão', () => {
  cy.validar_baixar_meus_certificados()
})

Quando('preencho o campo {string} com {string} nos certificados', (opcao, valor) => {
  cy.preencher_filtro_meus_certificados(opcao, valor)
})

Então('busca na listagem de Meus Certificados com {string}', (campo) => {
  cy.validar_filtros_meus_certificados(campo)
})

Quando('removo os filtros nos certificados obtidos nas formações', () => { 
  cy.limpar_filtros_meus_certificados()
})

Então('limpa os filtros em meus certificados', () => {
  cy.validar_sem_filtros_meus_certificados()
})
