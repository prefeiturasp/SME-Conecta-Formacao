import { When, Then } from '@badeball/cypress-cucumber-preprocessor'

const Quando = When
const Então = Then

Então('mostra o {string} no filtro {string} de nova inscrição disponíveis', (campo, valor) => {
  cy.validar_filtros_nova_inscricao(campo, valor)
})

Então('carrega o {string} das próximas formações disponíveis', () => { 
  cy.validar_proximas_formacoes() 
})

Então('carrega os detalhes {string} das próximas formações disponíveis', () => {  
  cy.validar_detalhes_formacoes()
})

Então('clico em {string} nova inscrição disponível', (campo) => {
  cy.enviar_proximas_formacoes()  
})

Então('retorna no {string} nova inscrição disponível', (campo) => {
  cy.cancelar_proximas_formacoes()  
})

Então('retorna que {string} é obrigatório em nova inscrição disponível', (campo) => {
  cy.validar_campos_obrigatorios_proximas_formacoes(campo) 
})