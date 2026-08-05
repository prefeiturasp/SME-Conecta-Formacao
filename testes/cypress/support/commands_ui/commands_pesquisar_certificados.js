import Pesquisar_Certificados_Localizadores from '../locators/pesquisar_certificados_locators'

const pesquisar_certificados_localizadores = new Pesquisar_Certificados_Localizadores()

Cypress.Commands.add('acessar_pesquisar_certificados', () => {
  cy.get(pesquisar_certificados_localizadores.menu_formacoes(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.get(pesquisar_certificados_localizadores.menu_lista_presenca())
    .contains('Pesquisar certificados')
    .click()

  cy.url({ timeout: 30000 })
    .should('include', 'certificados-pesquisa')
})

Cypress.Commands.add('filtrar_certificados', (situacao) => {
  cy.get(pesquisar_certificados_localizadores.btn_filtrar(), { timeout: 30000 })
    .should('to.exist')
    .click()

  cy.url({ timeout: 30000 })
    .should('include', 'certificados-pesquisa')
})

Cypress.Commands.add('baixar_certificado', () => {
  cy.get(pesquisar_certificados_localizadores.check(), { timeout: 30000 })
    .should('to.exist')
    .click()

  cy.contains(pesquisar_certificados_localizadores.btn_baixar_certificado(),
   'Baixar certificado', { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.get(pesquisar_certificados_localizadores.msg_sucesso(), { timeout: 30000 })
    .should('to.exist')
    .and('contain.text', 'O certificado foi baixado com sucesso.')
})

Cypress.Commands.add('baixar_todos_certificados', () => {
  cy.get(pesquisar_certificados_localizadores.check_todos(), { timeout: 30000 })
    .should('to.exist')
    .click()

  cy.contains(pesquisar_certificados_localizadores.btn_baixar_certificado(),
   'Baixar certificado', { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.get(pesquisar_certificados_localizadores.msg_sucesso(), { timeout: 30000 })
    .should('to.exist')
    .and('contain.text', 'Os certificados selecionados foram baixados com sucesso.')
})

Cypress.Commands.add('preencher_filtro_pesquisa_certificados', (opcao, valor, valorFinal = null) => {
  const campo = String(opcao).trim().toLowerCase()

  switch (campo) {
    case 'nome':
      cy.get(pesquisar_certificados_localizadores.campo_nome(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'tipo':
      cy.get(pesquisar_certificados_localizadores.select_tipo(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'código':
      cy.get(pesquisar_certificados_localizadores.campo_codigo(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'número':
      cy.get(pesquisar_certificados_localizadores.campo_numero(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break
    
    case 'certificado':
      cy.get(pesquisar_certificados_localizadores.campo_certificado(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'documento':
      cy.get(pesquisar_certificados_localizadores.campo_documento(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'regente':
      cy.get(pesquisar_certificados_localizadores.campo_regente(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'cursista':
      cy.get(pesquisar_certificados_localizadores.campo_cursista(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'data':
      cy.get(pesquisar_certificados_localizadores.campo_emissao(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)

      cy.get(pesquisar_certificados_localizadores.campo_emissao(), { timeout: 10000 })
        .should('be.visible')
        .click()        
      break

    case 'diretoria':
      cy.get(pesquisar_certificados_localizadores.select_dre(), { timeout: 10000 })
        .should('to.exist')
        .click()

      cy.get(pesquisar_certificados_localizadores.select_dre(valor), { timeout: 10000 })
        .should('to.exist')
        .click()  
      break
  
    default:
      throw new Error(`Campo "${opcao}" não mapeado`)
  }

  cy.get(pesquisar_certificados_localizadores.btn_filtrar(), { timeout: 10000 })
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_tabela_certificados', (campo) => {
  cy.get(pesquisar_certificados_localizadores.tbl_certificados(), { timeout: 10000 })
    .should('be.visible')
})

Cypress.Commands.add('nao_filtrar_certificado', (situacao) => {
  cy.get(pesquisar_certificados_localizadores.campo_nome_formacao(), { timeout: 10000 })
      .should('to.exist')
      .type('Nome Inexistente')

  cy.get(pesquisar_certificados_localizadores.btn_filtrar(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.url({ timeout: 30000 })
    .should('include', 'certificados-pesquisa')
})

Cypress.Commands.add('validar_sem_dados_certificados', () => {
  cy.contains('Não encontramos registros para os filtros aplicados')
    .should('be.visible')
})
