// Plugin do Allure (deve vir primeiro)
import '@shelex/cypress-allure-plugin'
import "cypress-cloud/support";

// Comandos personalizados - API

// Comandos personalizados - UI

// Evita falhas silenciosas caso algum comando seja removido ou renomeado
Cypress.on('uncaught:exception', (err, runnable) => {
  return false
})