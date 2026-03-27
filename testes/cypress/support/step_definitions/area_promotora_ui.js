import { Given, When, Then } from '@badeball/cypress-cucumber-preprocessor'

const Dado = Given
const Quando = When
const Então = Then


Quando('visualizo a tela {string}', (tela) => {
  if (tela === 'Área Promotora') {
    cy.clicar_tela_area_promotora()
  }
})

Quando('seleciono o tipo {string}', (tipo) => {
  cy.selecionar_tipo_area_promotora(tipo)
})

Quando('preencho o nome da promotora com {string}', (nome) => {
  cy.selecionar_nome_area_promotora(nome)
})

Então('sistema apresenta o resultado da consulta de área promotora', () => {
  cy.validar_resultado_area_promotora()
})

Então('sistema apresenta o resultado da consulta de área promotora com tipo {string}', (tipo) => {
  cy.validar_resultado_area_promotora_por_tipo(tipo)
})

Então('sistema apresenta o resultado da consulta de área promotora com {string}', (nome) => {
  cy.validar_resultado_area_promotora_por_nome(nome)
})

Quando('clico em "Novo" em Cadastro da Área Promotora do tipo {string}', (tipo) => {
  cy.criar_area_promotora_por_tipo(tipo)
})

Então('sistema cadastra área promotora dos tipos', () => {
})

Quando('clico em "Novo" em Cadastro da Área Promotora', () => {
  cy.criar_sem_dados_area_promotora()
})

Então('sistema não permite cadastrar área promotora com campos obrigatórios vazios', () => {
  cy.validar_campos_area_promotora()
})

Quando('clico no cadastro da Área Promotora', () => { 
})

Quando('depois clico para excluir da promotora', () => {
  cy.excluir_area_promotora()
})

Então('sistema confirma a exclusão de área promotora', () => {
  cy.validar_exclusao_area_promotora()
})

Quando('depois cancelo a exclusão da promotora', () => {
  cy.cancelar_exclusao_area_promotora() 
})

Então('sistema não confirma a exclusão de área promotora', () => {
  cy.validar_cancelamento_exclusao_area_promotora()
})

Quando('depois tento a exclusão da promotora', () => {
  cy.tentar_excluir_area_promotora()
})

Então('sistema não exclui área promotora com proposta cadastrada', () => {
  cy.validar_nao_exclusao_area_promotora()
})

Quando('depois clico na edição da promotora', () => {
  cy.editar_area_promotora()
})

Então('sistema edita a área promotora', () => {
  cy.validar_exclusao_area_promotora()
})

