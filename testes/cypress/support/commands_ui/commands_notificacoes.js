import Notificacao_Localizadores from '../locators/notificacao_locators'

const notificacao_localizadores = new Notificacao_Localizadores()

Cypress.Commands.add('acessar_notificacoes', () => {
  cy.get(notificacao_localizadores.opcao_notificacoes(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.url().should('include', '/notificacoes')
})

Cypress.Commands.add('validar_acesso_notificacoes', (campo) => {
  cy.url().should('contain', '/notificacoes')
})

Cypress.Commands.add('preencher_campos_notificacoes', (tipo, valor, valorFinal = null) => {
  const campo = String(tipo).trim().toLowerCase()

  switch (campo) {
    case 'código':   
      cy.get(notificacao_localizadores.campo_codigo(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'tipo':
      cy.get(notificacao_localizadores.selecionar_tipo(), { timeout: 10000 })
        .should('be.visible')
        .click()

      cy.contains(valor)
        .click()

      break

    case 'categoria':
      cy.get(notificacao_localizadores.selecionar_categoria(), { timeout: 10000 })
        .should('be.visible')
        .click()

      cy.contains(valor)
        .click()

      break  
      
    case 'título':
      cy.get(notificacao_localizadores.campo_titulo(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)

      break   
    
    case 'situação':
      cy.get(notificacao_localizadores.selecionar_situacao(), { timeout: 10000 })
        .should('be.visible')
        .click()

      cy.contains(valor)
        .click()

      break     

    default:
      throw new Error(`Campo "${tipo}" não mapeado`)
  }
})

Cypress.Commands.add('validar_filtros_notificacoes', (campo) => {
  cy.get(notificacao_localizadores.tbl_notificacoes(), { timeout: 10000 })
    .should('be.visible')
})
