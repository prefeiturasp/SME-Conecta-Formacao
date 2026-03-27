// ALLURE (sempre primeiro)
import '@shelex/cypress-allure-plugin'

// CUCUMBER (necessário para registrar steps corretamente)
import '@badeball/cypress-cucumber-preprocessor'

// COMANDOS CUSTOM - API
import './commands_api/commands_login'

// COMANDOS CUSTOM - UI
import './commands_ui/commands_login'
import './commands_ui/commands_area_promotora'

// Evita quebra por erro de front
Cypress.on('uncaught:exception', () => {
  return false
})