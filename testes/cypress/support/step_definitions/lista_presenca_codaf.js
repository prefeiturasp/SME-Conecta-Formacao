import { When, Then } from '@badeball/cypress-cucumber-preprocessor'

const Quando = When
const Então = Then

Quando('acesso o menu Lista Presença Codaf', () => {
  cy.acessar_lista_presenca()
})

Quando('filtro a presença nas formações', () => {
  cy.filtrar_lista_presenca()
})

Então('o sistema permite baixar o TXT CODAF', (situacao) => {
  cy.validar_baixar_lista_presenca_eol(situacao)
})

Então('o sistema permite baixar o relatório CODAF', (situacao) => {
  cy.validar_baixar_lista_presenca_codaf(situacao)
})

Quando('preencho o campo {string} com {string} na presença das formações', (opcao, valor) => {
  cy.preencher_filtro_lista_presenca(opcao, valor)
})

Então('busca na Lista Presença Codaf com {string}', (campo) => {
  cy.validar_filtros_lista_presenca(campo)
})

Quando('filtro dado não existente na presença nas formações', () => {
  cy.nao_filtrar_lista_presenca()
})

Então('o sistema informa dados não encontrados ao baixar o relatório CODAF', () => {
  cy.validar_sem_dados_lista_presenca()
})

Quando('removo os filtros na Lista Presença Codaf', () => { 
  cy.limpar_filtros_lista_presenca()
})

Então('limpa na presença nas formações', () => {
  cy.validar_sem_filtros_lista_presenca()
})
