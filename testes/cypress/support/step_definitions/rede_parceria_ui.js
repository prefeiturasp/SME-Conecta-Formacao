import { When, Then } from '@badeball/cypress-cucumber-preprocessor'

const Quando = When
const Então = Then


Quando('visualizo a tela Rede de Parceria', () => { 
  cy.clicar_tela_rede_parceria()
})

Quando('clico em "Novo" em Listagem de usuários para {string}', (situacao) => {
  cy.criar_novo_usuario(situacao)
  
  cy.excluir_usuario_rede_parceria(situacao) 
})

Então('sistema cadastra usuário de rede de parceria', () => {
  cy.validar_cadastro_rede_parceria()
})

Quando('clico em "Novo" em Listagem de usuários de parceria', () => {  
})

Então('sistema não permite cadastrar usuários de parceria com campos obrigatórios vazios', () => {
  cy.validar_campos_criar_novo_usuario()
})

Então('o sistema exibe o {string} no cadastro de usuário de rede de parceria', (campo) => {
  cy.validar_campos_cadastro_usuario(campo)
})

Quando('clico para excluir o usuário {string} da listagem', (situacao) => {

  cy.criar_novo_usuario(situacao)

  cy.excluir_usuario_rede_parceria() 
})

Então('sistema exclue o usuário de rede de parceria', () => {
  cy.validar_cadastro_rede_parceria() 
})

Quando('clico para cancelar a exclusão do usuário {string}', (situacao) => {
  cy.criar_novo_usuario(situacao)

  cy.cancelar_exclusao_usuario_rede_parceria()

  cy.excluir_usuario_rede_parceria() 
})

Então('sistema não exclui usuário de rede de parceria', () => {
  cy.validar_cadastro_rede_parceria()
})

Quando('clico na Listagem de usuários {string}', (situacao) => {
  cy.criar_novo_usuario(situacao)  
})

Então('sistema consulta o usuário de rede de parceria', () => {
  cy.consulta_usuario_rede_parceria() 

  cy.excluir_usuario_rede_parceria()
})

Quando('clico para consultar em Listagem de usuários de parceria {string}', (situacao) => {
  cy.criar_novo_usuario(situacao)

  cy.consulta_usuario_rede_parceria()  
})

Então('o sistema exibe o {string} preenchido na tela do usuário de rede de parceria', () => {
  cy.validar_campos_preenchidos_usuario_rede_parceria()

  cy.excluir_usuario_rede_parceria()
})

Quando('possuo cadastro em Listagem de usuários de parceria {string}', (situacao) => {
  cy.criar_novo_usuario(situacao)
})

Então('sistema filtra o usuário de rede de parceria', () => {
  cy.filtrar_usuario_rede_parceria()

  cy.excluir_usuario_rede_parceria()
})

Quando('edito o usuário de parceria {string}', (situacao) => {
  cy.criar_novo_usuario(situacao)
})

Então('sistema salva a alteração do usuário de parceria', () => {
  cy.editar_usuario_rede_parceria()

  cy.excluir_usuario_rede_parceria()
})

Quando('cancelo a edição o usuário de parceria {string}', (situacao) => {
  cy.criar_novo_usuario(situacao)
})

Então('sistema não salva a alteração do usuário de parceria', () => {
  cy.cancelar_edicao_usuario_rede_parceria()

  cy.excluir_usuario_rede_parceria()
})