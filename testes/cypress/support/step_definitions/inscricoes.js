import { When, Then } from '@badeball/cypress-cucumber-preprocessor'

const Quando = When
const Então = Then

Quando('acesso o menu Inscrições', () => {
  cy.acessar_inscricoes()
})

Quando('preencho o campo {string} com {string} nas inscrições', (tipo, valor) => {
  cy.preencher_campos_inscricoes(tipo, valor)
})

Então('exibe os campos de Inscrições {string}', (tipo) => {
  cy.validar_campos_inscricoes(tipo)
})

Então('busca na listagem em inscrições {string}', (campo) => {
  cy.validar_filtros_inscricoes(campo)
})

Quando('seleciono a formação em Inscrições', () => {
  cy.selecionar_formacao_inscricoes()
})

Quando('clico no botão de nova inscrição', () => {
  cy.realizar_inscricao_manual()
})

Então('realiza a inscrição manual', () => {
  cy.validar_inscricao_manual()
})

Então('informa que o cursista já está matriculado', () => {
  cy.validar_cursista_cadastrado_inscricao_manual()  
})

Quando('clico no botão de nova inscrição com usuário inexistente', () => {
  cy.realizar_inscricao_invalida_manual() 
})

Então('informa que o cursista é inválido', () => {
  cy.validar_inscricao_invalida_manual()
})

Quando('clico no botão de cancelar inscrição', () => {
  cy.cancelar_inscricao_manual()
})

Então('realiza o cancelamento da inscrição manual', () => {
  cy.validar_cancelamento_inscricao_manual() 
})

Quando('clico no botão de reativar inscrição', () => {
  cy.reativar_inscricao_manual()  
})

Então('reativa como confirmada a inscrição manual', () => {
  cy.validar_cursista_reativado_inscricao_manual()
  
  cy.cancelar_inscricao_manual()
})

Quando('clico no botão de espera na inscrição', () => {
  cy.espera_inscricao_manual()  
})

Então('realiza a espera da inscrição manual', () => {
  cy.validar_cursista_espera_inscricao_manual()

})

Quando('clico no botão de confirmar inscrição', () => {
  cy.confirmar_inscricao_manual()
})

Então('realiza a confirmação da inscrição manual', () => {
  cy.validar_cursista_confirmar_inscricao_manual()
})

Quando('tento enviar uma nova inscrição', () => {
})

Então('valida o campo obrigatório na inscrição manual', () => {
  cy.validar_campo_obrigatorio_inscricao_manual()
})