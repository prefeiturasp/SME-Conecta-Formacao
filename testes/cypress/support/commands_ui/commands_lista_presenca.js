import Lista_Presenca_Localizadores from '../locators/lista_presenca_locators'

const lista_presenca_localizadores = new Lista_Presenca_Localizadores()

Cypress.Commands.add('acessar_lista_presenca', () => {
  cy.get(lista_presenca_localizadores.menu_formacoes(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.get(lista_presenca_localizadores.menu_lista_presenca())
    .contains('Lista de Presença')
    .should('be.visible')
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
    .type('359')

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
    .should('exist')
    .click()

  cy.get(lista_presenca_localizadores.msg_sucesso(), { timeout: 30000 })
    .should('exist')
    .and('contain.text', 'Sucesso')
})

Cypress.Commands.add('validar_baixar_lista_presenca_codaf', () => {
  cy.get(lista_presenca_localizadores.btn_acoes(), { timeout: 30000 })
    .eq(1)
    .click()

  cy.contains(lista_presenca_localizadores.btn_gerar_arquivo(), 'Baixar Relatório CODAF', { timeout: 30000 })
    .should('exist')
    .click()

  cy.get(lista_presenca_localizadores.msg_sucesso(), { timeout: 30000 })
    .should('exist')
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
        .closest('.ant-select')  
        .should('exist')
        .click()

      cy.get(lista_presenca_localizadores.select_situacao(valor), { timeout: 10000 })
        .should('exist')
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
      .should('be.visible')
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

Cypress.Commands.add('criar_lista_presenca', () => {

  cy.get(lista_presenca_localizadores.btn_novo_registro(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.get(lista_presenca_localizadores.btn_continuar_registro(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.get(lista_presenca_localizadores.campo_homologacao(), { timeout: 30000 })
    .should('be.visible')
    .first()
    .type('123', { scrollBehavior: false })

  cy.get(lista_presenca_localizadores.opcao_situacao(), { timeout: 30000 })
    .contains('123')
    .should('be.visible')
    .click({ scrollBehavior: false })

  cy.intercept('GET', '**/api/v1/CodafListaPresenca/turmas/*/possui-lista').as('listar_turmas')

  cy.get(lista_presenca_localizadores.campo_turma(), { timeout: 30000 })
    .click({ scrollBehavior: false })

  cy.wait('@listar_turmas', { timeout: 30000 })
    .its('response.statusCode')
    .should('eq', 200)

  cy.get(lista_presenca_localizadores.opcao_turma(), { timeout: 30000 })
    .should('be.visible')
    .first()
    .click({ scrollBehavior: false })

  cy.get(lista_presenca_localizadores.btn_salvar(), { timeout: 30000 })
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_cadastro_lista_presenca', () => {
  cy.contains('Registro salvo com sucesso!')
    .should('be.visible')
})

Cypress.Commands.add('excluir_lista_presenca', () => {

  cy.get(lista_presenca_localizadores.campo_nome(), { timeout: 30000 })
    .should('be.visible')
    .type('Teste automação')

  cy.get(lista_presenca_localizadores.btn_filtrar(), { timeout: 30000 })
    .should('be.visible')
    .click()  

  cy.get(lista_presenca_localizadores.tbl_lista_presenca(), { timeout: 30000 })
    .first()
    .should('be.visible')
    .click()

  cy.get(lista_presenca_localizadores.btn_excluir(), { timeout: 30000 })
    .should('be.visible')
    .click()
    
  cy.get(lista_presenca_localizadores.btn_confirmar_exclusao(), { timeout: 30000 })
    .should('be.visible')
    .click()  
})

Cypress.Commands.add('validar_exclusao_lista_presenca', () => {
  cy.contains('Registro excluído com sucesso!')
    .should('be.visible')
})