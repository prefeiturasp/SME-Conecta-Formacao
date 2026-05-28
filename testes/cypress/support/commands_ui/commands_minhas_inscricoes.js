import Inscricoes_Localizadores from '../locators/inscricoes_locators'

const inscricoes_localizadores = new Inscricoes_Localizadores()

Cypress.Commands.add('acessar_minhas_inscricoes', () => {

  cy.contains('Minhas Inscrições', { timeout: 10000 })
    .should('be.visible')
})

Cypress.Commands.add('preencher_campos_minhas_inscricoes', (tipo, valor, valorFinal = null) => {
  const campo = String(tipo).trim().toLowerCase()

  switch (campo) {
    case 'código':
    case 'codigo':
      cy.get(inscricoes_localizadores.input_codigo(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'nome':
      cy.get(inscricoes_localizadores.input_nome(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'data':
      cy.get(inscricoes_localizadores.select_data(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'turma':
      cy.get(inscricoes_localizadores.input_turma_digitavel(), { timeout: 10000 })
        .should('exist')
        .clear()
        .type(valor, )
      break

    case 'período':
    case 'periodo':
      if (!valorFinal) {
        throw new Error('Para o campo "periodo" é necessário informar data inicial e data final')
      }

      cy.get(inscricoes_localizadores.input_periodo_inicial(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(`${valor}{enter}`)

      cy.get(inscricoes_localizadores.input_periodo_final(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(`${valorFinal}{enter}`)
      break

    case 'situação':
    case 'situacao':
      cy.get(inscricoes_localizadores.input_situacao(), { timeout: 10000 })
        .should('exist')
        .parents('.ant-select')
        .first()
        .find(inscricoes_localizadores.ant_select_selector())
        .should('be.visible')
        .click()

      cy.get(inscricoes_localizadores.ant_select_opcoes_visiveis(), { timeout: 10000 })
        .should('have.length.greaterThan', 0)
        .then(($opcoes) => {
          const opcoes = [...$opcoes].map(op => op.innerText.trim()).filter(Boolean)
          expect(
            opcoes,
            `Opções disponíveis para situação: ${opcoes.join(' | ')}`
          ).to.include(valor)
        })

      cy.contains(inscricoes_localizadores.ant_select_opcoes_visiveis(), valor, { timeout: 10000 })
        .should('be.visible')
        .click()
      break

    default:
      throw new Error(`Campo "${tipo}" não mapeado`)
  }
})

Cypress.Commands.add('validar_campos_minhas_inscricoes', (tipo) => {
  const campo = String(tipo).trim().toLowerCase()

  switch (campo) {
    case 'código':
    case 'codigo':
      cy.get(inscricoes_localizadores.input_codigo(), { timeout: 10000 })
        .should('be.visible')
      break

    case 'nome':
      cy.get(inscricoes_localizadores.input_nome(), { timeout: 10000 })
        .should('be.visible')
      break

    case 'data':
      cy.get(inscricoes_localizadores.select_data(), { timeout: 10000 })
        .should('be.visible')
      break

    case 'turma':
      cy.get(inscricoes_localizadores.input_turma(), { timeout: 10000 })
        .should('be.visible')
      break

    case 'período':
    case 'periodo':
      cy.get(inscricoes_localizadores.input_periodo_inicial(), { timeout: 10000 })
        .should('be.visible')

      cy.get(inscricoes_localizadores.input_periodo_final(), { timeout: 10000 })
        .should('be.visible')
      break

    case 'situação':
    case 'situacao':
      cy.get(inscricoes_localizadores.input_situacao(), { timeout: 10000 })
        .should('exist')
        .parents('.ant-select')
        .first()
        .find(inscricoes_localizadores.ant_select_selector())
        .should('be.visible')
      break

    default:
      throw new Error(`Campo "${tipo}" não mapeado`)
  }
})

Cypress.Commands.add('acessar_aba_finalizadas_minhas_inscricoes', () => {
  cy.intercept('GET', '**/api/v1/Inscricao/finalizadas**').as('getInscricoesFinalizadas')

  cy.get(inscricoes_localizadores.tbl_finalizadas(), { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.wait('@getInscricoesFinalizadas').its('response.statusCode').should('eq', 200)
})

Cypress.Commands.add('preencher_campos_minhas_inscricoes_finalizadas', (tipo, valor, valorFinal = null) => {
  cy.acessar_aba_finalizadas_minhas_inscricoes()

  const campo = String(tipo).trim().toLowerCase()

  switch (campo) {
    case 'nome':
      cy.get(inscricoes_localizadores.input_nome_finalizada(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'situação':
    case 'situacao':
      cy.get(inscricoes_localizadores.input_situacao_finalizada(), { timeout: 10000 })
        .should('exist')
        .parents('.ant-select')
        .first()
        .find(inscricoes_localizadores.ant_select_selector())
        .should('be.visible')
        .click()

      cy.get(inscricoes_localizadores.ant_select_opcoes_visiveis(), { timeout: 10000 })
        .should('have.length.greaterThan', 0)
        .then(($opcoes) => {
          const opcoes = [...$opcoes].map(op => op.innerText.trim()).filter(Boolean)
          expect(
            opcoes,
            `Opções disponíveis para situação: ${opcoes.join(' | ')}`
          ).to.include(valor)
        })

      cy.contains(inscricoes_localizadores.ant_select_opcoes_visiveis(), valor, { timeout: 10000 })
        .should('be.visible')
        .click()
      break

    case 'período':
    case 'periodo':
      if (!valorFinal) {
        throw new Error('Para o campo "periodo" é necessário informar data inicial e data final')
      }

      cy.get(inscricoes_localizadores.input_periodo_inicial_finalizada(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(`${valor}{enter}`)

      cy.get(inscricoes_localizadores.input_periodo_final_finalizada(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(`${valorFinal}{enter}`)
      break

    default:
      throw new Error(`Campo "${tipo}" não mapeado para inscrições finalizadas`)
  }
})

Cypress.Commands.add('validar_campos_minhas_inscricoes_finalizadas', (tipo) => {
  cy.acessar_aba_finalizadas_minhas_inscricoes()

  const campo = String(tipo).trim().toLowerCase()

  switch (campo) {
    case 'nome':
      cy.get(inscricoes_localizadores.input_nome_finalizada(), { timeout: 10000 })  
        .first()
        .should('be.visible')
      break

    case 'situação':
    case 'situacao':
      cy.get(inscricoes_localizadores.input_situacao_finalizada(), { timeout: 10000 })
        .first()
        .should('exist')
        .parents('.ant-select')
        .first()
        .find(inscricoes_localizadores.ant_select_selector())
        .should('be.visible')
      break

    case 'período':
    case 'periodo':
      cy.get(inscricoes_localizadores.input_periodo_inicial_finalizada(), { timeout: 10000 })
        .first()
        .should('be.visible')

      cy.get(inscricoes_localizadores.input_periodo_final_finalizada(), { timeout: 10000 })
        .first()
        .should('be.visible')
      break

    default:
      throw new Error(`Campo "${tipo}" não mapeado para inscrições finalizadas`)
  }
})

Cypress.Commands.add('clicar_explorar_formacoes', () => {
  cy.get(inscricoes_localizadores.btn_explorar_formacoes(), { timeout: 10000 })
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_explorar_formacoes', () => {
  cy.contains('Nova inscrição', { timeout: 10000 })
    .should('be.visible')  
})