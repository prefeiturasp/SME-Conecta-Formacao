// ALLURE
import '@shelex/cypress-allure-plugin'

// CUCUMBER
import '@badeball/cypress-cucumber-preprocessor'

// COMANDOS - API
import './commands_api/commands_login'

// COMANDOS - UI
import './commands_ui/commands_login'
import './commands_ui/commands_area_promotora'
import './commands_ui/commands_meus_dados'
import './commands_ui/commands_minhas_inscricoes'
import './commands_ui/commands_rede_parceria'
import './commands_ui/commands_nova_inscricao'
import './commands_ui/commands_inscricoes'
import './commands_ui/commands_redefinir_senha'

// Evita quebra por erro de front
Cypress.on('uncaught:exception', () => {
  return false
})