import { When, Then } from '@badeball/cypress-cucumber-preprocessor'

const Quando = When
const Então = Then

Quando('acesso o menu Minhas Inscrições', () => {
  cy.acessar_minhas_inscricoes()
})

Quando('preencho o campo {string} com {string} nas inscriçõees ativas', (tipo, valor) => {
  cy.preencher_campos_minhas_inscricoes(tipo, valor)
})

Então('retorna o período com {string} e {string} em Minhas Inscrições', (dataInicial, dataFinal) => {
  cy.preencher_campos_minhas_inscricoes('periodo', dataInicial, dataFinal)
})

Então('exibe os campos de Minhas Inscrições em andamento {string}', (tipo) => {
  cy.validar_campos_minhas_inscricoes(tipo)
})

Quando('preencho o campo {string} com {string} nas formações concluídas', (tipo, valor) => {
  cy.preencher_campos_minhas_inscricoes_finalizadas(tipo, valor)
})

Quando('preencho o período com {string} e {string} nas Inscrições finalizadas', (dataInicial, dataFinal) => {
  cy.preencher_campos_minhas_inscricoes_finalizadas('periodo', dataInicial, dataFinal)
})

Então('exibe os campos de Minhas Inscrições finalizadas {string}', (tipo) => {
  cy.validar_campos_minhas_inscricoes_finalizadas(tipo)
})

Então('exibe os campos de Minhas Inscrições em finalizadas {string}', (tipo) => {
  cy.validar_campos_minhas_inscricoes_finalizadas(tipo)
})

Quando('clico em explorar formações', () => {
  cy.clicar_explorar_formacoes() 
})

Então('exibe para consulta de novas formações', () => {
  cy.validar_explorar_formacoes()
})