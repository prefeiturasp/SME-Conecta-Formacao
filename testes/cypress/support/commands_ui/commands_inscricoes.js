import Meus_Dados_Localizadores from '../locators/meus_dados_locators'
import Inscricoes_Localizadores from '../locators/inscricoes_locators'

const meus_dados_localizadores = new Meus_Dados_Localizadores()
const inscricoes_localizadores = new Inscricoes_Localizadores()

Cypress.Commands.add('acessar_minhas_inscricoes', () => {

  cy.contains('Minhas Inscrições', { timeout: 10000 })
    .should('be.visible')
})

Cypress.Commands.add('validar_campo_meus_dados', (campo) => {
  const campoNormalizado = String(campo).trim().toLowerCase()

  function validarInputPreenchido(selector, nomeCampo) {
    cy.get(selector, { timeout: 10000 })
      .should('be.visible')
      .should(($el) => {
        const valor =
          $el.prop('value') ||
          $el.attr('value') ||
          $el.val() ||
          $el.text()

        expect(String(valor).trim(), nomeCampo).to.not.equal('')
      })
  }

  switch (campoNormalizado) {
    case 'nome':
      validarInputPreenchido(meus_dados_localizadores.input_nome(), 'Campo Nome')
      break

    case 'email':
      validarInputPreenchido(meus_dados_localizadores.input_email(), 'Campo Email')
      break

    case 'tipo':
      cy.get(meus_dados_localizadores.select_tipo(), { timeout: 10000 })
        .should('be.visible')
        .invoke('text')
        .then((texto) => {
          expect(texto.trim(), 'Campo Tipo').to.not.equal('')
        })
      break

    case 'email educacional':
      validarInputPreenchido(
        meus_dados_localizadores.input_email_educacional(),
        'Campo Email Educacional'
      )
      break

    case 'pessoa deficiencia':
      cy.get(meus_dados_localizadores.select_pessoa_deficiencia(), { timeout: 10000 })
        .should('be.visible')
        .invoke('text')
        .then((texto) => {
          expect(texto.trim(), 'Campo Pessoa com deficiência').to.not.equal('')
        })
      break

    case 'senha':
      cy.get(meus_dados_localizadores.input_senha(), { timeout: 10000 })
        .should('be.visible')
      break

    default:
      throw new Error(`Campo de Meus Dados não tratado: ${campo}`)
  }
})

Cypress.Commands.add('clicar_alterar', (campo) => {
  const campoNormalizado = String(campo).trim().toLowerCase()

  switch (campoNormalizado) {
    case 'nome':
      cy.get(meus_dados_localizadores.btn_alterar_nome(), { timeout: 10000 })
        .should('have.length.greaterThan', 0)
        .first()
        .should('be.visible')
        .scrollIntoView()
        .click()
      break

    case 'email':
    case 'e-mail':
      cy.get(meus_dados_localizadores.btn_alterar_email(), { timeout: 10000 })
        .should('have.length.greaterThan', 0)
        .first()
        .should('be.visible')
        .scrollIntoView()
        .click()
      break

    case 'senha':
      cy.get(meus_dados_localizadores.btn_alterar_senha(), { timeout: 10000 })
        .should('have.length.greaterThan', 0)
        .first()
        .should('be.visible')
        .scrollIntoView()
        .click()
      break

    default:
      throw new Error(`Campo não tratado: ${campo}`)
  }
})

Cypress.Commands.add('clicar_salvar_modal', () => {
  cy.intercept('PUT', '**/api/v1/usuario/**').as('putUsuario')

  cy.get(meus_dados_localizadores.btn_modal_alterar(), { timeout: 10000 })
    .should('exist')
    .should('be.visible')
    .should('not.be.disabled')
    .click()

  cy.wait('@putUsuario', { timeout: 15000 }).then((interception) => {
    expect(interception.response.statusCode).to.be.oneOf([200, 204])
  })

  cy.get(meus_dados_localizadores.msgm_alteracao_sucesso(), { timeout: 10000 })
    .should('exist')
    .should('be.visible')

  cy.get('body').then(($body) => {
    if ($body.find(meus_dados_localizadores.btn_modal_alterar()).length > 0) {
      cy.get(meus_dados_localizadores.btn_modal_alterar()).should('not.be.visible')
    } else {
      cy.get(meus_dados_localizadores.btn_modal_alterar()).should('not.exist')
    }
  })
})

Cypress.Commands.add('clicar_modal_cancelar', () => {
  cy.get(meus_dados_localizadores.btn_modal_cancelar(), { timeout: 10000 })
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_modal_alteracao_visivel', () => {
  cy.get(meus_dados_localizadores.btn_modal_alterar(), { timeout: 10000 })
    .should('exist')
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_modal_alteracao_nao_visivel', () => {
  cy.get(meus_dados_localizadores.btn_modal_alterar(), { timeout: 10000 })
    .should('not.exist')
})

Cypress.Commands.add('validar_alteracao_meus_dados', () => {
  cy.get(meus_dados_localizadores.msgm_alteracao_sucesso(), { timeout: 10000 })
    .should('to.visible')
})

Cypress.Commands.add('validar_campos_modal_senha', (campo) => {
  const campoNormalizado = String(campo)
    .trim()
    .toLowerCase()

  let selector

  switch (campoNormalizado) {
    case 'senha atual':
      selector = meus_dados_localizadores.input_senha_atual()
      break

    case 'nova senha':
      selector = meus_dados_localizadores.input_nova_senha()
      break

    case 'confirmação senha':
    case 'confirmacao senha':
      selector = meus_dados_localizadores.input_confirmacao_senha()
      break

    default:
      throw new Error(`Campo não mapeado no modal: ${campo}`)
  }

  cy.get('.ant-modal-content', { timeout: 10000 })
    .should('be.visible')
    .within(() => {
      cy.get(selector)
        .should('exist')
        .and('be.visible')
    })
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