import Lista_Presenca_Localizadores from '../locators/lista_presenca_locators'

const lista_presenca_localizadores = new Lista_Presenca_Localizadores()

Cypress.Commands.add('acessar_lista_presenca', () => {
  cy.get(lista_presenca_localizadores.menu_formacoes(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.contains(lista_presenca_localizadores.menu_lista_presenca(), 'Lista de Presença CODAF')
    .click()

  cy.contains(lista_presenca_localizadores.menu_lista_presenca(), 'Formações homologadas')
    .click()

  cy.get(lista_presenca_localizadores.menu_lista_presenca())
    .contains('Formações homologadas')
    .click() 
  
  cy.url({ timeout: 30000 })
    .should('include', 'lista-presenca-codaf')
})

Cypress.Commands.add('filtrar_lista_presenca', (situacao) => {
  cy.get(lista_presenca_localizadores.select_situacao(), { timeout: 10000 })
    .click()

  cy.contains(
    lista_presenca_localizadores.opcao_situacao(), situacao, { timeout: 10000 })
    .should('exist')
    .click()
  
  cy.get(lista_presenca_localizadores.campo_codigo(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type('140')

  cy.get(lista_presenca_localizadores.btn_filtrar(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.url({ timeout: 30000 })
    .should('include', 'lista-presenca-codaf')
})

Cypress.Commands.add('validar_baixar_lista_presenca_eol', () => {
  cy.get(lista_presenca_localizadores.btn_acoes(), { timeout: 30000 })
    .eq(1)
    .click()

  cy.contains(lista_presenca_localizadores.btn_gerar_arquivo(), 'Gerar TXT EOL', { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.get(lista_presenca_localizadores.msg_sucesso(), { timeout: 30000 })
    .should('be.visible')
    .and('contain.text', 'Sucesso')
})

Cypress.Commands.add('validar_baixar_lista_presenca_codaf', () => {
  cy.get(lista_presenca_localizadores.btn_acoes(), { timeout: 30000 })
    .eq(1)
    .click()

  cy.contains(lista_presenca_localizadores.btn_gerar_arquivo(), 'Baixar Relatório CODAF', { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.get(lista_presenca_localizadores.msg_sucesso(), { timeout: 30000 })
    .should('be.visible')
    .and('contain.text', 'Sucesso')
})

Cypress.Commands.add('preencher_filtro_lista_presenca', (opcao, valor, valorFinal = null) => {
  const campo = String(opcao).trim().toLowerCase()

  switch (campo) {
    case 'nome':
      cy.get(lista_presenca_localizadores.campo_nome(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'área':
      cy.get(lista_presenca_localizadores.select_area(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'código':
      cy.get(lista_presenca_localizadores.campo_codigo(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'número':
      cy.get(lista_presenca_localizadores.campo_homologacao(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'data':
      cy.get(lista_presenca_localizadores.campo_envio(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)

      cy.get(lista_presenca_localizadores.campo_envio(), { timeout: 10000 })
        .should('be.visible')
        .click()        
      break

    case 'situação':
      cy.get(lista_presenca_localizadores.select_situacao(), { timeout: 10000 })
        .should('to.exist')
        .click()

      cy.get(lista_presenca_localizadores.select_situacao(valor), { timeout: 10000 })
        .should('to.exist')
        .click()  
      break
  
    default:
      throw new Error(`Campo "${opcao}" não mapeado`)
  }

  cy.get(lista_presenca_localizadores.btn_filtrar(), { timeout: 10000 })
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_filtros_lista_presenca', (campo) => {
  cy.get(lista_presenca_localizadores.tbl_lista_presenca(), { timeout: 10000 })
    .should('be.visible')
})

Cypress.Commands.add('nao_filtrar_lista_presenca', (situacao) => {
  cy.get(lista_presenca_localizadores.campo_nome(), { timeout: 10000 })
      .should('to.exist')
      .type('Nome Inexistente')

  cy.get(lista_presenca_localizadores.btn_filtrar(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.url({ timeout: 30000 })
    .should('include', 'lista-presenca-codaf')
})

Cypress.Commands.add('validar_sem_dados_lista_presenca', () => {
  cy.contains('Não encontramos registros para os filtros aplicados')
    .should('be.visible')
})

Cypress.Commands.add('limpar_filtros_lista_presenca', () => {
  cy.get(lista_presenca_localizadores.btn_limpar(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.url({ timeout: 30000 })
    .should('include', 'lista-presenca-codaf')
})

Cypress.Commands.add('validar_sem_filtros_lista_presenca', () => {
  cy.contains('Não encontramos registros para os filtros aplicados').should('exist')
})

