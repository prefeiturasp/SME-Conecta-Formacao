const { defineConfig } = require('cypress')
const allureWriter = require('@shelex/cypress-allure-plugin/writer')
const dotenv = require('dotenv')

const createBundler = require('@bahmutov/cypress-esbuild-preprocessor')
const { addCucumberPreprocessorPlugin } = require('@badeball/cypress-cucumber-preprocessor')
const createEsbuildPlugin = require('@badeball/cypress-cucumber-preprocessor/esbuild').default

const postgreSQL = require('cypress-postgresql')
const pg = require('pg')
const fs = require('fs')
const FormData = require('form-data')
const axios = require('axios')

dotenv.config()

const dbConfig = {
  user: process.env.DB_USER,
  password: process.env.DB_PASSWORD,
  host: process.env.DB_HOST,
  database: process.env.DB_DATABASE,
}

module.exports = defineConfig({
  e2e: {

    baseUrl: 'https://hom-conectaformacao.sme.prefeitura.sp.gov.br',

    viewportWidth: 1920,
    viewportHeight: 1080,

    specPattern: 'cypress/e2e/**/*.feature',

    supportFile: 'cypress/support/e2e.js',

    env: {

      LOGIN_ADM_GERAL: process.env.LOGIN_ADM_GERAL,
      LOGIN_CURSISTA: process.env.LOGIN_CURSISTA,
      LOGIN_EXTERNO: process.env.LOGIN_EXTERNO,
      CPF: process.env.CPF,
      NOME: process.env.NOME,
      EMAIL: process.env.EMAIL,
      SENHA: process.env.SENHA,
      ID_AREA_PROMOTORA: process.env.ID_AREA_PROMOTORA,
      PERFIL_AREA_PROMOTORA: process.env.PERFIL_AREA_PROMOTORA,
      LABEL_AREA_PROMOTORA: process.env.LABEL_AREA_PROMOTORA,
      VALUE_AREA_PROMOTORA: process.env.VALUE_AREA_PROMOTORA,
      TELEFONES: process.env.TELEFONES,
      GRUPO_AREA_PROMOTORA: process.env.GRUPO_AREA_PROMOTORA,
      EMAIL_DOMAIN: process.env.EMAIL_DOMAIN,
      PROPOSTA_ID: process.env.PROPOSTA_ID,
      PROPOSTA_TURMA_ID: process.env.PROPOSTA_TURMA_ID,
      CARGO_CODIGO: process.env.CARGO_CODIGO,
      CARGO_DRE_CODIGO: process.env.CARGO_DRE_CODIGO,
      CARGO_UE_CODIGO: process.env.CARGO_UE_CODIGO,
      TIPO_VINCULO: process.env.TIPO_VINCULO,   

      cucumber: {
        stepDefinitions: "cypress/support/step_definitions/**/*.js"
      }
    },

    async setupNodeEvents(on, config) {

      await addCucumberPreprocessorPlugin(on, config)

      on(
        "file:preprocessor",
        createBundler({
          plugins: [createEsbuildPlugin(config)],
        })
      )

      allureWriter(on, config)

      const pool = new pg.Pool(dbConfig)
      const dbTasks = postgreSQL.loadDBPlugin(pool)

      on('task', {
        ...dbTasks,

        async uploadFile({ method = 'POST', url, headers = {}, filePath }) {

          const form = new FormData()

          if (filePath) {
            form.append('file', fs.createReadStream(filePath))
          }

          const response = await axios({
            method,
            url,
            headers: {
              ...headers,
              ...form.getHeaders(),
            },
            data: form,
            maxBodyLength: Infinity,
            validateStatus: () => true,
          })

          return {
            status: response.status,
            body: response.data,
          }
        },
      })

      return config
    },
  },
})