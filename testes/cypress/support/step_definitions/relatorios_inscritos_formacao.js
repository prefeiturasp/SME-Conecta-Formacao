import { When, Then } from '@badeball/cypress-cucumber-preprocessor'

const Quando = When
const Então = Then

Quando('acesso o menu Relatório de inscritos por formação', () => {
  cy.acessar_menu_relatorios_inscritos_formacao()
})

Quando('preencho para gerar o relatório de inscritos', () => {
  cy.preencher_relatorio_inscritos_formacao()  
})

Então('gera o relatório com sucesso de inscritos por formação', () => {
  cy.validar_gera_relatorio_inscritos_formacao()
  
})

Quando('não preencho para gerar o relatório de inscritos', () => {  
})

Então('não gera o relatório de inscritos por formação', () => {
  cy.validar_nao_gera_relatorio_inscritos_formacao()  
})

Então('retorna os campos de inscritos por formação para {string}', (campo) => {
  cy.validar_campo_relatorio_inscritos_formacao(campo)
})


