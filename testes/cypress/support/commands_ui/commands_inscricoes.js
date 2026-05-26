import Common_Localizadores from '../locators/common_locators'
import Inscricao_Localizadores from '../locators/inscricao_locators'

const common_Localizadores = new Common_Localizadores()
const inscricao_localizadores = new Inscricao_Localizadores()

Cypress.Commands.add('acessar_inscricoes', () => {
  cy.contains(common_Localizadores.menu_formacoes(), 'Formações', { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.contains(inscricao_localizadores.menu_inscricoes(), 'Inscrições', { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.url().should('include', '/formacoes/inscricoes')
})

Cypress.Commands.add('preencher_campos_inscricoes', (tipo, valor, valorFinal = null) => {
  const campo = String(tipo).trim().toLowerCase()

  switch (campo) {
    case 'código':   
      cy.get(inscricao_localizadores.campo_codigo(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'nome':
      cy.get(inscricao_localizadores.campo_nome(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break

    case 'homologação':
      cy.get(inscricao_localizadores.campo_homologacao(), { timeout: 10000 })
        .should('be.visible')
        .clear()
        .type(valor)
      break    

    default:
      throw new Error(`Campo "${tipo}" não mapeado`)
  }
})

Cypress.Commands.add('validar_campos_inscricoes', (tipo) => {
  const campo = String(tipo).trim().toLowerCase()

  switch (campo) {
    case 'código':    
      cy.get(inscricao_localizadores.campo_codigo(), { timeout: 10000 })
        .should('be.visible')
      break

    case 'nome':
      cy.get(inscricao_localizadores.campo_nome(), { timeout: 10000 })
        .should('be.visible')
      break

    case 'homologação':
      cy.get(inscricao_localizadores.campo_homologacao(), { timeout: 10000 })
        .should('be.visible')
      break

    default:
      throw new Error(`Campo "${tipo}" não mapeado`)
  }
})

Cypress.Commands.add('validar_filtros_inscricoes', (campo) => {

  cy.get(inscricao_localizadores.listagem_cursos(), { timeout: 10000 })
    .should('be.visible')
    .click()

  const filtro = campo.toLowerCase().trim()

  switch (filtro) {

    case 'turma':
      cy.get(inscricao_localizadores.abrir_turma())         
        .click()

      cy.get(inscricao_localizadores.selecionar_turma('TURMA 01'))
        .first()
        .click()
      break

    case 'cargo':
      cy.get(inscricao_localizadores.selecionar_cargo()).click()

      cy.contains('PROF. ED. INF. E ENS. FUND.')
        .click()
      break

    case 'situação':
      cy.get(inscricao_localizadores.selecionar_situacao())
        .click()

      cy.contains('Cancelada')
        .click()
      break

    case 'rf':
      cy.get(inscricao_localizadores.campo_registro())
        .clear()
        .type(Cypress.env('LOGIN_ADM_GERAL'))
      break

    case 'documento':
      cy.get(inscricao_localizadores.campo_documento())
        .clear()
        .type(Cypress.env('CPF'))
      break

    case 'nome':
      cy.get(inscricao_localizadores.campo_nome_cursista())
        .clear()
        .type(Cypress.env('NOME'))
      break

    default:
      throw new Error(`Campo "${campo}" não mapeado`)
  }

  cy.url()
    .should('include', '/formacoes/inscricoes')
})

Cypress.Commands.add('selecionar_formacao_inscricoes', () => {
  cy.get(inscricao_localizadores.campo_codigo(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type('344')

  cy.get(inscricao_localizadores.listagem_cursos(), { timeout: 10000 })
    .should('be.visible')
    .click()   
})

Cypress.Commands.add('realizar_inscricao_manual', () => {
  cy.get(inscricao_localizadores.btn_nova_inscricao(), { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.get(inscricao_localizadores.selecionar_turma_cronograma(), { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.contains(inscricao_localizadores.selecionar_turma_1(), 'Turma 1', { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.get(inscricao_localizadores.campo_rf(), { timeout: 10000 })
    .should('be.visible')
    .type(Cypress.env('LOGIN_ADM_GERAL'))

  cy.get(inscricao_localizadores.buscar_rf(), { timeout: 10000 })
    .should('be.visible')
    .first()
    .click()

  cy.get(inscricao_localizadores.selecionar_cargo_inscricao())
    .should('be.visible')
    .click()
    .type('PROF.ED.INF.E ENS.FUND.I - v1{enter}')

  cy.get(inscricao_localizadores.salvar_inscricao(), { timeout: 10000 })
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_inscricao_manual', () => {
  cy.get(inscricao_localizadores.msg_inscricao(), { timeout: 15000 })
    .should('be.visible')
    .and('contain.text', 'Inscrição manual realizada com sucesso!')
})

Cypress.Commands.add('realizar_inscricao_invalida_manual', () => {
  cy.get(inscricao_localizadores.btn_nova_inscricao(), { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.get(inscricao_localizadores.selecionar_turma_cronograma(), { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.contains(inscricao_localizadores.selecionar_turma_1(), 'Turma 1', { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.get(inscricao_localizadores.campo_rf(), { timeout: 10000 })
    .should('be.visible')
    .type(Cypress.env('CARGO_DRE_CODIGO'))

  cy.get(inscricao_localizadores.buscar_rf(), { timeout: 10000 })
    .should('be.visible')
    .first()
    .click()

  cy.get(inscricao_localizadores.selecionar_cargo_inscricao())
    .should('be.visible')
    .click()
    .type('PROF.ED.INF.E ENS.FUND.I - v1{enter}')

  cy.get(inscricao_localizadores.salvar_inscricao(), { timeout: 10000 })
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_inscricao_invalida_manual', () => {
  cy.get(inscricao_localizadores.msg_inscricao(), { timeout: 15000 })
    .should('be.visible')
})

Cypress.Commands.add('validar_campo_obrigatorio_inscricao_manual', () => {
  cy.get(inscricao_localizadores.btn_nova_inscricao(), { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.get(inscricao_localizadores.salvar_inscricao(), { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.get(inscricao_localizadores.msg_obrigatorio(), { timeout: 10000 })
    .should('be.visible')  
  })

Cypress.Commands.add('espera_inscricao_manual', () => {  
  cy.get(inscricao_localizadores.selecionar_situacao())
    .click()

  cy.contains('Aguardando Análise', { timeout: 15000 })
    .click()

  cy.get(inscricao_localizadores.campo_registro())
    .clear()
    .type(Cypress.env('LOGIN_ADM_GERAL'))

  cy.get(inscricao_localizadores.esperar_inscricao(), { timeout: 10000 })
    .eq(2)
    .should('be.visible')
    .should('be.enabled')

  cy.get(inscricao_localizadores.esperar_inscricao())
    .eq(2)
    .click()

  cy.get(inscricao_localizadores.confirmar_modal(), { timeout: 10000 })
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_cursista_espera_inscricao_manual', () => {
  cy.get(inscricao_localizadores.msg_inscricao(), { timeout: 15000 })
    .should('be.visible')
    .and('contain.text', 'Inscrições colocadas Em Espera com sucesso!')

  cy.reload()
})

Cypress.Commands.add('confirmar_inscricao_manual', () => {  
  cy.get(inscricao_localizadores.selecionar_situacao())
    .click()

  cy.contains('Em Espera', { timeout: 15000 })
    .click()

  cy.get(inscricao_localizadores.campo_registro())
    .clear()
    .type(Cypress.env('LOGIN_ADM_GERAL'))

  cy.get(inscricao_localizadores.confirmar_inscricao(), { timeout: 10000 })
    .eq(3)
    .should('be.visible')
    .should('be.enabled')

  cy.get(inscricao_localizadores.confirmar_inscricao())
    .eq(3)
    .click()

  cy.get(inscricao_localizadores.confirmar_modal(), { timeout: 10000 })
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_cursista_confirmar_inscricao_manual', () => {
  cy.get(inscricao_localizadores.msg_inscricao(), { timeout: 15000 })
    .should('be.visible')
    .and('contain.text', 'Inscrições confirmadas com sucesso!')

  cy.reload()
})

Cypress.Commands.add('cancelar_inscricao_manual', () => {  
  cy.get(inscricao_localizadores.selecionar_situacao())
    .click()

  cy.contains('Confirmada')
    .click()

  cy.get(inscricao_localizadores.campo_registro())
    .clear()
    .type(Cypress.env('LOGIN_ADM_GERAL'))
    
  cy.get(inscricao_localizadores.cancelar_inscricao(), { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.get(inscricao_localizadores.confirmar_modal(), { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.get(inscricao_localizadores.motivo_cancelamento(), { timeout: 10000 })
    .should('be.visible')
    .type('Teste automatizado')

  cy.get(inscricao_localizadores.botao_cancelar_inscricao(), { timeout: 10000 })
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_cancelamento_inscricao_manual', () => {
  cy.get(inscricao_localizadores.msg_inscricao(), { timeout: 15000 })
    .should('be.visible')
    .and('contain.text', 'Inscrições canceladas com sucesso!')
})

Cypress.Commands.add('validar_cursista_cadastrado_inscricao_manual', () => {
  cy.get(inscricao_localizadores.msg_inscricao(), { timeout: 15000 })
    .should('be.visible')
    .and('contain.text', 'Este cursista já está matriculado nesta formação.')
})

Cypress.Commands.add('reativar_inscricao_manual', () => {  
  cy.get(inscricao_localizadores.selecionar_situacao())
    .click()

  cy.contains('Cancelada', { timeout: 10000 })
    .click()

  cy.get(inscricao_localizadores.campo_registro())
    .clear()
    .type(Cypress.env('LOGIN_ADM_GERAL'))
    
  cy.get(inscricao_localizadores.reativar_inscricao(), { timeout: 10000 })
    .should('be.visible')
    .first()
    .click() 
  
  cy.get(inscricao_localizadores.confirmar_modal(), { timeout: 10000 })
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_cursista_reativado_inscricao_manual', () => {
  cy.get(inscricao_localizadores.msg_inscricao(), { timeout: 15000 })
    .should('be.visible')
    .and('contain.text', 'Reativação confirmada com sucesso!')

  cy.reload()
})