import { defineConfig } from 'cypress'
import allureWriter from '@shelex/cypress-allure-plugin/writer.js'
import { cloudPlugin } from 'cypress-cloud/plugin'
import dotenv from 'dotenv'

import createBundler from '@bahmutov/cypress-esbuild-preprocessor'
import { addCucumberPreprocessorPlugin } from '@badeball/cypress-cucumber-preprocessor'
import { createEsbuildPlugin } from '@badeball/cypress-cucumber-preprocessor/esbuild'

import postgreSQL from 'cypress-postgresql'
import pg from 'pg'
import fs from 'fs'
import path from 'path'
import FormData from 'form-data'
import axios from 'axios'

dotenv.config()

const dbConfig = {
  user: process.env.DB_USER,
  password: process.env.DB_PASSWORD,
  host: process.env.DB_HOST,
  database: process.env.DB_DATABASE,
}

export default defineConfig({
  e2e: {

    watchForFileChanges: true,

    baseUrl: 'https://hom-conectaformacao.sme.prefeitura.sp.gov.br',

    viewportWidth: 1920,
    viewportHeight: 1080,

    specPattern: ['cypress/e2e/**/*.feature'],

    supportFile: 'cypress/support/e2e.js',

    video: false,
    retries: { runMode: 2, openMode: 0 },
    screenshotOnRunFailure: false,
    chromeWebSecurity: false,
    experimentalRunAllSpecs: true,
    failOnStatusCode: false,

    defaultCommandTimeout: 60000,
    requestTimeout: 60000,
    execTimeout: 60000,
    pageLoadTimeout: 60000,
    waitForAnimations: true,
    animationDistanceThreshold: 5,

    async setupNodeEvents(on, config) {

      // Cucumber
      await addCucumberPreprocessorPlugin(on, config)

      on(
        'file:preprocessor',
        createBundler({
          plugins: [createEsbuildPlugin(config)],
        })
      )

      // Allure
      allureWriter(on, config)

      // Banco de dados
      const pool = new pg.Pool(dbConfig)
      const dbTasks = postgreSQL.loadDBPlugin(pool)

      on('task', {
        ...dbTasks,

      // Limpa a pasta cypress/downloads
        clearDownloads() {
          const downloadsFolder = path.join(process.cwd(), 'cypress', 'downloads')

          if (fs.existsSync(downloadsFolder)) {
            fs.readdirSync(downloadsFolder).forEach((file) => {
              fs.unlinkSync(path.join(downloadsFolder, file))
            })
          }

          return null
        },

        async uploadFile({ method = 'POST', url, headers = {}, filePath }) {

          const form = new FormData()

          if (filePath && filePath.trim() !== '') {
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

      const envKeys = [
        'LOGIN_ADM_GERAL',
        'LOGIN_CURSISTA',
        'LOGIN_EXTERNO',
        'CPF',
        'NOME',
        'EMAIL',
        'SENHA',
        'TOKEN_RECUPERACAO',
        'ID_AREA_PROMOTORA',
        'PERFIL_AREA_PROMOTORA',
        'LABEL_AREA_PROMOTORA',
        'VALUE_AREA_PROMOTORA',
        'TELEFONES',
        'GRUPO_AREA_PROMOTORA',
        'EMAIL_DOMAIN',
        'PROPOSTA_ID',
        'PROPOSTA_TURMA_ID',
        'CARGO_CODIGO',
        'CARGO_DRE_CODIGO',
        'CARGO_UE_CODIGO',
        'TIPO_VINCULO',
        'CODAF_ID',
        'CERTIFICADO_CODAF_ID',
        'CERTIFICADO_ID',
        'REGISTRO_FUNCIONAL'
      ]

      const customEnv = Object.fromEntries(
        envKeys.map((key) => [key, process.env[key] ?? ''])
      )

      config.env = {
        ...config.env,
        ...customEnv,
        cucumber: {
          stepDefinitions: 'cypress/support/step_definitions/**/*.js'
        },
        db: dbConfig
      }

      // Cypress Cloud
      const enhancedConfig = await cloudPlugin(on, config)

      return enhancedConfig
    },
  },
})