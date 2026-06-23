import Nova_Inscricao_Localizadores from '../locators/nova_inscricao_locators'

const nova_inscricao_localizadores = new Nova_Inscricao_Localizadores()

Cypress.Commands.add('validar_filtros_nova_inscricao', (campo, valor) => {

  const filtros = {
    'público': nova_inscricao_localizadores.select_publico_alvo(),
    'título': nova_inscricao_localizadores.input_titulo(),
    'área': nova_inscricao_localizadores.select_area_promotora(),
    'data': nova_inscricao_localizadores.select_data(),
    'formato': nova_inscricao_localizadores.select_formato(),
    'palavra': nova_inscricao_localizadores.select_palavras_chave()
  }

  cy.get(filtros[campo], { timeout: 10000 })
    .should('be.visible')
    .click()

  if (campo === 'título' || campo === 'data') {

    cy.get(filtros[campo])
      .clear()
      .type(valor)

  } else {

    cy.contains(valor)
      .should('be.visible')
      .click()
  }

  cy.get(nova_inscricao_localizadores.btn_buscar_formacoes())
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_proximas_formacoes', () => {
  cy.get(nova_inscricao_localizadores.card_proximas_formacoes(), { timeout: 10000 })
    .should('have.length.greaterThan', 0)
    .first()
    .should('be.visible')
})

Cypress.Commands.add('validar_detalhes_formacoes', () => {
  cy.get(nova_inscricao_localizadores.btn_detalhes_formacoes(), { timeout: 10000 })
    .should('have.length.greaterThan', 0)
    .first()
    .should('be.visible')
    .click()

  cy.url({ timeout: 10000 }).should('include', 'area-publica/visualizar')
})

Cypress.Commands.add('enviar_proximas_formacoes', () => {
  cy.intercept('GET', '**/api/v1/Inscricao/turmas/**').as('getTurmas')
  cy.intercept('POST', '**/api/v1/Inscricao').as('postInscricao')

  cy.gerar_token().then((token) => {
    cy.get(nova_inscricao_localizadores.btn_detalhes_formacoes())
      .should('be.visible')
      .first()
      .click()
    
    cy.get(nova_inscricao_localizadores.btn_enviar_inscricao())
      .should('be.visible')
      .click()

    cy.wait('@getTurmas')

    cy.get(nova_inscricao_localizadores.input_turma(), { timeout: 10000 })
      .should('exist')
      .should('not.be.disabled')

    cy.get(nova_inscricao_localizadores.input_turma())
      .click()

    cy.get(nova_inscricao_localizadores.input_turma())
      .type('{enter}')

    cy.get(nova_inscricao_localizadores.input_deficiencia(), { timeout: 10000 })
      .should('exist')
      .should('not.be.disabled')
      .type('Não{enter}', { force: true })

    cy.get(nova_inscricao_localizadores.btn_nova_inscricao())
      .should('be.visible')
      .click()

    cy.get(nova_inscricao_localizadores.btn_salvar_informacoes())
      .should('be.visible')
      .click()

    cy.wait('@postInscricao').then(({ response }) => {
      const idInscricao = response.body.entidadeId

      cy.log(`ID inscrição: ${idInscricao}`)

      cy.request({
        method: 'PUT',
        url: `${Cypress.config('baseUrl')}/api/v1/Inscricao/${idInscricao}/cancelar`,
        headers: {
          accept: 'text/plain',
          Authorization: `Bearer ${token}`
        }
      }).then((response) => {
        expect(response.status).to.eq(200)
        cy.log(`Inscrição ${idInscricao} cancelada com sucesso`)
      })
    })

    cy.contains('Sua inscrição foi', { timeout: 10000 })
      .should('be.visible')
  })
})

Cypress.Commands.add('cancelar_proximas_formacoes', () => {
  cy.get(nova_inscricao_localizadores.btn_detalhes_formacoes(), { timeout: 10000 })
    .should('be.visible')
    .first()
    .click()

  cy.get(nova_inscricao_localizadores.btn_enviar_inscricao(), { timeout: 10000 })
    .should('be.visible') 
    .click()

  cy.get(nova_inscricao_localizadores.btn_voltar(), { timeout: 10000 })
    .should('be.visible') 
    .click()

  cy.get(nova_inscricao_localizadores.btn_nao_enviar_informacoes(), { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.url({ timeout: 10000 }).should('include', 'area-publica')  
})

Cypress.Commands.add('validar_campos_obrigatorios_proximas_formacoes', (campo) => {
  cy.get(nova_inscricao_localizadores.btn_detalhes_formacoes(), { timeout: 10000 })
    .should('be.visible')
    .first()
    .click()

  cy.get(nova_inscricao_localizadores.btn_enviar_inscricao(), { timeout: 10000 })
    .should('be.visible') 
    .click()

  cy.get(nova_inscricao_localizadores.btn_nova_inscricao())
    .should('be.visible')
    .click()

  cy.url({ timeout: 10000 }).should('include', 'inscricao') 
})