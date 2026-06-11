import { Given } from "@badeball/cypress-cucumber-preprocessor"

const Dado = Given

Dado('eu acesso o sistema com a visualização web', function () {
    cy.vizualicacao_login()
})

Dado('realizo login no sistema Conecta Formação com perfil {string}', function (perfil) {
    cy.realizar_login(perfil)
})