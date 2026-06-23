import Meus_Certificados_Localizadores from '../locators/meus_certificados_locators'

const meus_certificados_localizadores = new Meus_Certificados_Localizadores()

Cypress.Commands.add('acessar_meus_certificados', () => {
  cy.get(meus_certificados_localizadores.menu_meus_certificados(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.get(meus_certificados_localizadores.opcao_meus_certificados())
    .contains('Meus Certificados')
    .click()

  cy.url({ timeout: 30000 })
    .should('include', 'certificados')
})

Cypress.Commands.add('validar_baixar_meus_certificados', (campo) => {
  cy.url({ timeout: 30000 })
    .should('contain', 'certificados')

  cy.contains('Ação').should('to.exist')
})

Cypress.Commands.add('filtrar_meus_certificados', () => {
  cy.get(meus_certificados_localizadores.btn_filtrar(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.url({ timeout: 30000 })
    .should('include', 'certificados')
})

Cypress.Commands.add('preencher_filtro_meus_certificados', (opcao, valor, valorFinal = null) => {
  const campo = String(opcao).trim().toLowerCase()

  switch (campo) {
    case 'código':
      cy.get(meus_certificados_localizadores.campo_homologacao(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'nome':
      cy.get(meus_certificados_localizadores.campo_nome(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'data':
      cy.get(meus_certificados_localizadores.campo_emissao(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)

      cy.get(meus_certificados_localizadores.campo_codigo(), { timeout: 10000 })
        .should('be.visible')
        .click()        
      break

    case 'número':
      cy.get(meus_certificados_localizadores.campo_codigo(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'tipo':
     cy.get(meus_certificados_localizadores.select_tipo(), { timeout: 10000 })
        .should('to.exist')
        .click()

      cy.contains(valor)
        .click()
      break

    default:
      throw new Error(`Campo "${opcao}" não mapeado`)
  }

  cy.get(meus_certificados_localizadores.btn_filtrar(), { timeout: 10000 })
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_filtros_meus_certificados', (campo) => {
  cy.get(meus_certificados_localizadores.tbl_meus_certificados(), { timeout: 10000 })
    .should('be.visible')
})

Cypress.Commands.add('limpar_filtros_meus_certificados', () => {
  cy.get(meus_certificados_localizadores.btn_limpar(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.url({ timeout: 30000 })
    .should('include', 'certificados')
})

Cypress.Commands.add('validar_sem_filtros_meus_certificados', () => {
  cy.contains('Ação').should('not.exist')
})