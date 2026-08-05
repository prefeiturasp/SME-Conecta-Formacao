import Rede_Parceria_Localizadores from '../locators/rede_parceria_locators'
import Common_Localizadores from '../locators/common_locators'

const rede_parceria_Localizadores = new Rede_Parceria_Localizadores()
const common_Localizadores = new Common_Localizadores()

Cypress.Commands.add('clicar_tela_rede_parceria', () => {
  cy.contains(common_Localizadores.menu_cadastro(), 'Cadastro', { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.contains(rede_parceria_Localizadores.menu_rede_parceria(), 'Rede de Parceria (Usuários)', { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.contains('Listagem de usuários', { timeout: 15000 })
    .should('be.visible')

  cy.get(rede_parceria_Localizadores.btn_novo(), { timeout: 30000 })
    .should('be.visible')
})

Cypress.Commands.add('criar_novo_usuario', (situacao) => {
  cy.get(rede_parceria_Localizadores.btn_novo(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.get(rede_parceria_Localizadores.filtro_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .click()
    .type('Teste')

  cy.contains(rede_parceria_Localizadores.opcoes_area_promotora(), 'Teste', { timeout: 10000 })
    .should('be.visible')
    .click()
  
  cy.get(rede_parceria_Localizadores.input_cpf())
    .should('be.visible')
    .type(Cypress.env('CPF'))

  cy.get(rede_parceria_Localizadores.input_nome_usuario())
    .should('be.visible')
    .type('Teste Automatizado')
  
  cy.get(rede_parceria_Localizadores.input_email())
    .should('be.visible')
    .type(Cypress.env('EMAIL'))
  
  cy.get(rede_parceria_Localizadores.input_telefone(), { timeout: 30000 })
    .should('be.visible')
    .type(Cypress.env('TELEFONES'))
  
  cy.get(rede_parceria_Localizadores.select_situacao())
    .should('be.visible')
    .click()

  cy.contains(situacao, { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.get(rede_parceria_Localizadores.btn_salvar())
    .should('be.visible')
    .click()

  cy.get(rede_parceria_Localizadores.btn_confirmar_cadastro_usuario(), { timeout: 3000 })
    .should('be.visible')
    .click()

  cy.contains(
    'body',
    /Usuário salvo com sucesso!|Usuário já possui cadastro no Conecta como rede parceria/,
    { timeout: 10000 }
  ).then(($msg) => {
    const texto = $msg.text()

    if (texto.includes('Usuário já possui cadastro no Conecta como rede parceria')) {
      cy.excluir_usuario_rede_parceria(situacao)
      cy.wait(10000)
      cy.criar_novo_usuario(situacao)
    } else {
      cy.contains('Usuário salvo com sucesso!', { timeout: 10000 })
        .should('be.visible')
    }
  })
})

Cypress.Commands.add('validar_campos_criar_novo_usuario', () => {
  cy.get(rede_parceria_Localizadores.btn_novo(), { timeout: 15000 })
    .should('be.visible')
    .click()

  cy.get(rede_parceria_Localizadores.filtro_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .click()

  cy.contains('Teste', { timeout: 10000 })
    .should('be.visible')
    .click()  

  cy.get(rede_parceria_Localizadores.btn_salvar(), { timeout: 3000 })
    .should('be.visible')
    .click()

  cy.get(rede_parceria_Localizadores.msg_campo_obrigatorio(), { timeout: 3000 })
    .should('be.visible')  
})

Cypress.Commands.add('validar_cadastro_rede_parceria', () => {
  cy.contains('Listagem de usuários', { timeout: 15000 }).should('be.visible')

})

Cypress.Commands.add('validar_campos_cadastro_usuario', (campo) => {
  cy.get(rede_parceria_Localizadores.btn_novo(), { timeout: 15000 })
    .should('be.visible')
    .click()

  const campos = [
    rede_parceria_Localizadores.filtro_area_promotora(),
    rede_parceria_Localizadores.input_cpf(),
    rede_parceria_Localizadores.input_nome_usuario(),
    rede_parceria_Localizadores.input_email(),    
    rede_parceria_Localizadores.input_telefone(),
    rede_parceria_Localizadores.select_situacao()
  ]

  campos.forEach((seletor) => {
    cy.get(seletor, { timeout: 10000 })
      .should('be.visible')
      .and('exist')
  })
})

Cypress.Commands.add('excluir_usuario_rede_parceria', () => {
  cy.clicar_tela_rede_parceria()

  cy.get(rede_parceria_Localizadores.input_cpf(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type(Cypress.env('CPF'))

  cy.get(rede_parceria_Localizadores.tbl_cpf(), { timeout: 10000 })
    .should('be.visible')

  cy.get(rede_parceria_Localizadores.tbl_cpf())
    .first()
    .click()

  cy.get(rede_parceria_Localizadores.btn_excluir_usuario(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.get(rede_parceria_Localizadores.btn_confirmar_cadastro_usuario(), { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.url().should('include', 'cadastro/rede-parceria')
})

Cypress.Commands.add('cancelar_exclusao_usuario_rede_parceria', () => {
  cy.clicar_tela_rede_parceria()

  cy.get(rede_parceria_Localizadores.input_cpf(), { timeout: 30000 })
    .should('be.visible')
    .clear()
    .type(Cypress.env('CPF'))

  cy.get(rede_parceria_Localizadores.tbl_cpf(), { timeout: 30000 })
    .should('be.visible')

  cy.get(rede_parceria_Localizadores.tbl_cpf())
    .first()
    .click()

  cy.get(rede_parceria_Localizadores.btn_excluir_usuario(), { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.get(rede_parceria_Localizadores.btn_cancelar_exclusao_usuario(), { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.contains('Cadastro de novo usuário', { timeout: 10000 })
    .should('be.visible')
})

Cypress.Commands.add('consulta_usuario_rede_parceria', () => {
  cy.clicar_tela_rede_parceria()

  cy.get(rede_parceria_Localizadores.input_cpf(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type(Cypress.env('CPF'))

  cy.get(rede_parceria_Localizadores.tbl_cpf(), { timeout: 10000 })
    .should('be.visible')

  cy.get(rede_parceria_Localizadores.tbl_cpf())
    .first()
    .click() 

  cy.url().should('include', 'cadastro/rede-parceria')
})

Cypress.Commands.add('validar_campos_preenchidos_usuario_rede_parceria', () => {
    cy.get(rede_parceria_Localizadores.msg_campo_obrigatorio(), { timeout: 10000 })
    .should('not.exist')  
})

Cypress.Commands.add('filtrar_usuario_rede_parceria', () => {
  cy.clicar_tela_rede_parceria()

  cy.get(rede_parceria_Localizadores.filtro_area_promotora(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type('Teste')

  cy.get(rede_parceria_Localizadores.filtro_nome(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type('Teste Automatizado')

  cy.get(rede_parceria_Localizadores.input_cpf(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type(Cypress.env('CPF'))

  cy.get(rede_parceria_Localizadores.select_situacao(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type('Ativo')

  cy.get(rede_parceria_Localizadores.tbl_cpf(), { timeout: 10000 })
    .should('be.visible')

  cy.get(rede_parceria_Localizadores.tbl_cpf())
    .first()
    .click() 

  cy.url().should('include', 'cadastro/rede-parceria')
})

Cypress.Commands.add('editar_usuario_rede_parceria', (situacao) => {
  cy.intercept('GET', '**/api/v1/UsuarioRedeParceria/*').as('getUsuarioDetalhe')

  cy.clicar_tela_rede_parceria()

  cy.get(rede_parceria_Localizadores.input_cpf(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type(Cypress.env('CPF'))

  cy.get(rede_parceria_Localizadores.tbl_cpf(), { timeout: 10000 })
    .should('be.visible')
    .first()
    .click()

  cy.wait('@getUsuarioDetalhe')

  cy.get(rede_parceria_Localizadores.btn_salvar())
    .should('be.visible')
    .and('be.disabled')
})

Cypress.Commands.add('cancelar_edicao_usuario_rede_parceria', () => {
  cy.clicar_tela_rede_parceria()
  
  cy.get(rede_parceria_Localizadores.input_cpf(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type(Cypress.env('CPF'))

  cy.get(rede_parceria_Localizadores.tbl_cpf(), { timeout: 10000 })
    .should('be.visible')

  cy.get(rede_parceria_Localizadores.tbl_cpf(), { timeout: 10000 })
    .first()
    .click()  

  cy.get(rede_parceria_Localizadores.btn_voltar(), { timeout: 5000 })
    .should('be.visible')
    .click()

  cy.url().should('include', 'cadastro/rede-parceria')
})