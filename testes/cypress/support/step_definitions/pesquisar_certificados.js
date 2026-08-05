import { When, Then } from '@badeball/cypress-cucumber-preprocessor'

const Quando = When
const Então = Then

Quando('acesso o menu Pesquisar certificados', () => {
  cy.acessar_pesquisar_certificados()
})

Quando('filtro todos certificados na pesquisa', () => {
  cy.filtrar_certificados()
})

Então('o sistema exibe todos os dados de certificados', () => {
  cy.validar_tabela_certificados()
})

Quando('clico para baixar o certificado selecionado', () => {
  cy.baixar_certificado()
})

Quando('clico para baixar todos certificados selecionados', () => {
  cy.baixar_todos_certificados()
})

Então('o sistema realiza o download dos certificados', () => {
  cy.validar_tabela_certificados()
})

Quando('filtro dado de certificado inexistente na pesquisa ', () => {
  cy.nao_filtrar_certificado()
})

Então('o sistema sem dados de certificados', () => {
  cy.validar_sem_dados_certificados()
})

Quando('filtro o campo {string} de certificado com {string} na pesquisa', (opcao, valor) => {
  cy.preencher_filtro_pesquisa_certificados(opcao, valor)
})

Então('busca nas pequisa de certificado com {string}', (campo) => {
  cy.validar_tabela_certificados(campo)
})