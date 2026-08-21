import Inscritos_Formacao_Localizadores from '../locators/inscritos_formacao_locators'

const inscritos_formacao_localizadores = new Inscritos_Formacao_Localizadores()

Cypress.Commands.add('acessar_menu_relatorios_inscritos_formacao', () => { 

  cy.contains(inscritos_formacao_localizadores.submenu_relatorios_inscritos_formacao(), 'Relatórios', { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.contains(inscritos_formacao_localizadores.item_menu_relatorios_inscritos_formacao(), 'Inscritos por formação', { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.url().should('include', '/relatorios/inscritos-por-formacao')
})

Cypress.Commands.add('preencher_relatorio_inscritos_formacao', () => {

  const hoje = new Date()
  const dia = String(hoje.getDate()).padStart(2, '0')
  const mes = String(hoje.getMonth() + 1).padStart(2, '0')
  const ano = hoje.getFullYear()

  const dataAtual = `${dia}/${mes}/${ano}`

  cy.get(inscritos_formacao_localizadores.select_inicio(), { timeout: 10000 })
    .should('exist')
    .click()
    .clear()
    .type(dataAtual)
    .type('{enter}')

  cy.get(inscritos_formacao_localizadores.select_fim(), { timeout: 10000 })
    .should('exist')
    .click()
    .clear()
    .type(dataAtual)
    .type('{enter}')

  cy.contains('button', 'Próximo', { timeout: 10000 })
    .should('exist')
    .click()

  cy.contains('button', 'Próximo', { timeout: 10000 })
    .should('exist')
    .click()

  cy.contains('button', 'Gerar Relatório', { timeout: 10000 })
    .should('exist')
    .click()  
})

Cypress.Commands.add('validar_gera_relatorio_inscritos_formacao', () => {
  cy.get(inscritos_formacao_localizadores.messagem(), { timeout: 10000 })
    .should('contain.text', 'Sucesso')
})

Cypress.Commands.add('validar_nao_gera_relatorio_inscritos_formacao', () => {
  cy.contains('button', 'Gerar Relatório', { timeout: 10000 })
    .should('be.disabled')
  })

Cypress.Commands.add('validar_campo_relatorio_inscritos_formacao', (campo) => {
  const campoNormalizado = String(campo).trim().toLowerCase()

  function validarFiltro(selector) {
    cy.get(selector, { timeout: 10000 })
      .should('exist')
      .should(($el) => {
        const valor =
          $el.prop('value') ||
          $el.attr('value') ||
          $el.val() ||
          $el.text()

        expect(String(valor).trim()).to.not.equal('')
      })
  }

  switch (campoNormalizado) {
    case 'formação':
      cy.get(inscritos_formacao_localizadores.input_codigo_formacao(), { timeout: 10000 })
        .should('exist')
        .clear()
        .type('123')

      validarFiltro(
        inscritos_formacao_localizadores.input_codigo_formacao()
      )
      break

    case 'homologação':
      cy.get(inscritos_formacao_localizadores.input_codigo_homologacao(), { timeout: 10000 })
        .should('exist')
        .clear()
        .type('123')

      validarFiltro(
        inscritos_formacao_localizadores.input_codigo_homologacao()
      )
      break

    case 'turma':
      cy.get(inscritos_formacao_localizadores.select_turma(), { timeout: 10000 })
        .should('be.disabled')
      break

    case 'modalidade':
      cy.get(inscritos_formacao_localizadores.select_formato(), { timeout: 10000 })
        .should('exist')
       .click()

      cy.contains(inscritos_formacao_localizadores.select_opcao(), 'Presencial', { timeout: 10000 })
       .should('be.visible')
       .click()

      validarFiltro(
        inscritos_formacao_localizadores.select_opcao()
      )
      break

    case 'nome':
      cy.get(inscritos_formacao_localizadores.input_nome(), { timeout: 10000 })
        .should('exist')
        .clear()
        .type('123')

      validarFiltro(
        inscritos_formacao_localizadores.input_nome()
      )
      break

    case 'área':
      cy.get(inscritos_formacao_localizadores.select_area_promotora(), { timeout: 10000 })
        .should('exist')
        .click()
        .type('123')

      validarFiltro(
        inscritos_formacao_localizadores.select_area_promotora()
      )
      break

    case 'situação':
      cy.get(inscritos_formacao_localizadores.select_situacao(), { timeout: 10000 })
        .should('exist')
        .click()
      
      cy.contains(inscritos_formacao_localizadores.select_opcao(), 'Confirmada', { timeout: 10000 })
        .should('be.visible')
        .click()

      validarFiltro(
        inscritos_formacao_localizadores.select_opcao()
      )
      break

    default:
      throw new Error(`Campo não tratado: ${campo}`)
  }
})