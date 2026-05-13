import Area_Promotora_Localizadores from '../locators/area_promotora_locators'
import Common_Localizadores from '../locators/common_locators'

const area_promotora_localizadores = new Area_Promotora_Localizadores()
const common_Localizadores = new Common_Localizadores()

Cypress.Commands.add('clicar_tela_area_promotora', () => {
  cy.contains(common_Localizadores.menu_cadastro(), 'Cadastro', { timeout: 30000 })
    .should('be.visible')
    .click()

  cy.contains(area_promotora_localizadores.menu_area_promotora(), 'Área promotora', { timeout: 30000 })
  .should('be.visible')
  .click()

  cy.get(area_promotora_localizadores.input_nome(), { timeout: 3000 })
    .should('be.visible')

  cy.get(area_promotora_localizadores.select_tipo(), { timeout: 3000 })
    .should('be.visible')
})

Cypress.Commands.add('validar_resultado_area_promotora', () => {
  cy.get(area_promotora_localizadores.tbl_linhas(), { timeout: 3000 })
    .should('have.length.greaterThan', 0)
})

Cypress.Commands.add('selecionar_tipo_area_promotora', (tipo) => {
  cy.get(area_promotora_localizadores.select_tipo(), { timeout: 5000 })
    .eq(0)
    .should('be.visible')
    .click()

  cy.get(area_promotora_localizadores.lista_opcoes_tipo(), { timeout: 5000 })
    .should('be.visible')
    .contains(new RegExp(`^${tipo}$`))
    .click()

  cy.get(area_promotora_localizadores.tbl_linhas(), { timeout: 10000 })
    .should('have.length.greaterThan', 0)

  cy.get(area_promotora_localizadores.tbl_tipo(), { timeout: 10000 })
    .should('have.length.greaterThan', 0)
    .then(($cells) => {
      const textos = [...$cells].map((el) => el.innerText.trim())

      expect(
        textos.some((texto) => texto === tipo),
        `registro com "${tipo}", retornou: ${textos.join(', ')}`
      ).to.eq(true)
    })
})

Cypress.Commands.add('validar_resultado_area_promotora_por_tipo', (tipos) => {
  const tiposValidos = Array.isArray(tipos)
    ? tipos
    : tipos.split(',').map(t => t.trim())

  cy.get(area_promotora_localizadores.tbl_tipo(), { timeout: 10000 })
    .should('have.length.greaterThan', 0)
    .then(($cells) => {
      const valores = [...$cells].map((el) => el.innerText.trim())

      const encontrou = valores.some((valor) =>
        tiposValidos.some((tipo) => valor.includes(tipo))
      )

      expect(
        encontrou,
        `espera um dos tipos: ${tiposValidos.join(', ')}. Retornou: ${valores.join(', ')}`
      ).to.eq(true)
    })
})

Cypress.Commands.add('selecionar_nome_area_promotora', (nome) => {
  cy.get(area_promotora_localizadores.input_nome(), { timeout: 3000 })
    .should('be.visible')
    .clear()
    .type(nome)
})

Cypress.Commands.add('validar_resultado_area_promotora_por_nome', (nome) => {
  cy.get(area_promotora_localizadores.tbl_linhas(), { timeout: 3000 })
    .should('have.length.greaterThan', 0)

  cy.get(area_promotora_localizadores.tbl_nome(), { timeout: 3000 })
    .first()
    .invoke('text')
    .then((text) => {
      expect(text.trim().toLowerCase()).to.contain(nome.toLowerCase())
    })
})

Cypress.Commands.add('criar_area_promotora_por_tipo', (tipo) => {
  expect(tipo).to.be.oneOf([
    'Rede Parceria',
    'Rede Direta'
  ])

  const tipoTexto = String(tipo).trim()
  const nomeBase = 'Teste automatizado'

  cy.log(`tipo recebido=${tipoTexto}`)
  cy.log(`nomeBase=${nomeBase}`)

  function selecionarOpcaoAnt(inputSelector, textoOpcao) {
    const opcao = String(textoOpcao).trim()

    cy.log(`opção recebida=${opcao}`)
    expect(opcao, 'opção enviada').to.be.oneOf([
      'Rede Parceria',
      'Rede Direta',
      'Arquivo Histórico Municipal'
    ])

    cy.get(inputSelector, { timeout: 10000 })
      .should('exist')
      .parents('.ant-select')
      .first()
      .find('.ant-select-selector')
      .trigger('mousedown', { which: 1, force: true })

    cy.contains('.ant-select-item-option-content', new RegExp(`^${Cypress._.escapeRegExp(opcao)}$`), {
      timeout: 10000
    })
      .should('be.visible')
      .click()
  }

  const agora = new Date()
  const dia = String(agora.getDate()).padStart(2, '0')
  const mes = String(agora.getMonth() + 1).padStart(2, '0')
  const ano = agora.getFullYear()
  const hora = String(agora.getHours()).padStart(2, '0')
  const minuto = String(agora.getMinutes()).padStart(2, '0')

  const nomeAreaPromotora = `${nomeBase} ${dia}/${mes}/${ano} ${hora}:${minuto}`
  const emailTeste = `testeautomatizado${dia}${mes}${ano}${hora}${minuto}@sme.com`

  cy.get(area_promotora_localizadores.btn_salvar_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .click()

  cy.get(area_promotora_localizadores.btn_novo_nome_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .clear()
    .type(nomeAreaPromotora)

  selecionarOpcaoAnt(
    area_promotora_localizadores.btn_novo_tipo_area_promotora(),
    tipoTexto
  )

  selecionarOpcaoAnt(
    area_promotora_localizadores.btn_novo_perfil_area_promotora(),
    'Arquivo Histórico Municipal'
  )

  cy.get(area_promotora_localizadores.btn_novo_telefone_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .clear()
    .type('31999999999')

  cy.get(area_promotora_localizadores.btn_novo_email_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .clear()
    .type(emailTeste)
})

Cypress.Commands.add('excluir_area_promotora', (nomeFiltro = 'Teste automatizado') => {
  cy.get(area_promotora_localizadores.input_nome(), { timeout: 5000 })
    .should('be.visible')
    .clear()
    .type(nomeFiltro)

  cy.wait(1000)

  cy.get('body').then(($body) => {
    const linhaExiste = $body.find(area_promotora_localizadores.tbl_nome_teste()).length > 0

    if (linhaExiste) {
      cy.get(area_promotora_localizadores.tbl_nome_teste(), { timeout: 5000 })
        .should('be.visible')
        .click()

      cy.get(area_promotora_localizadores.btn_exclui_area_promotora(), { timeout: 5000 })
        .should('be.visible')
        .click()

      cy.get(area_promotora_localizadores.btn_confirmar_modal_area_promotora(), { timeout: 5000 })
        .should('be.visible')
        .click()

      cy.wait(1000)
    }
  })

  cy.get(area_promotora_localizadores.input_nome(), { timeout: 5000 })
    .should('be.visible')
    .clear()
})

Cypress.Commands.add('criar_sem_dados_area_promotora', () => {  

  cy.get(area_promotora_localizadores.btn_salvar_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .click()
  
  cy.contains('button', 'Salvar', { timeout: 5000 })
    .should('be.visible')
    .click()
})

Cypress.Commands.add('excluir_area_promotora', (tipo = 'Rede Parceria') => {
  expect(tipo).to.be.a('string').and.not.be.empty

  const tipoTexto = tipo.trim()

  const agora = new Date()
  const dia = String(agora.getDate()).padStart(2, '0')
  const mes = String(agora.getMonth() + 1).padStart(2, '0')
  const ano = agora.getFullYear()
  const hora = String(agora.getHours()).padStart(2, '0')
  const minuto = String(agora.getMinutes()).padStart(2, '0')

  const nomeAreaPromotora = `Teste automatizado ${dia}/${mes}/${ano} ${hora}:${minuto}`
  const emailTeste = `testeautomatizado${dia}${mes}${ano}${hora}${minuto}@sme.com`

  function selecionarOpcaoAnt(inputSelector, textoOpcao) {
    const opcao = String(textoOpcao).trim()

    cy.get(inputSelector, { timeout: 8000 })
      .should('exist')
      .parents('.ant-select')
      .first()
      .find('.ant-select-selector')
      .trigger('mousedown', { which: 1, force: true })

    cy.contains('.ant-select-item-option', opcao, { timeout: 8000 })
      .click()
  }

  cy.get(area_promotora_localizadores.btn_salvar_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .click()

  cy.get(area_promotora_localizadores.btn_novo_nome_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .clear()
    .type(nomeAreaPromotora)

  selecionarOpcaoAnt(area_promotora_localizadores.btn_novo_tipo_area_promotora(), tipoTexto)
  selecionarOpcaoAnt(area_promotora_localizadores.btn_novo_perfil_area_promotora(), 'Arquivo Histórico Municipal')

  cy.get(area_promotora_localizadores.btn_novo_telefone_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .clear()
    .type('31999999999')

  cy.get(area_promotora_localizadores.btn_novo_email_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .clear()
    .type(emailTeste)

  cy.contains('button', 'Salvar', { timeout: 5000 })
    .should('be.visible')
    .click()

  cy.get(area_promotora_localizadores.msg_sucesso_area_promotora(), { timeout: 5000 })
    .should('be.visible')

  cy.get(area_promotora_localizadores.input_nome(), { timeout: 5000 })
    .should('be.visible')
    .clear()
    .type(nomeAreaPromotora)

  cy.wait(1000)

  cy.get(area_promotora_localizadores.tbl_nome_teste(), { timeout: 5000 })
    .should('be.visible')
    .click()

  cy.get(area_promotora_localizadores.btn_exclui_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .click()

  cy.get(area_promotora_localizadores.btn_confirmar_modal_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .click()

  cy.wait(1000)

  cy.get(area_promotora_localizadores.input_nome(), { timeout: 5000 })
    .should('be.visible')
    .clear()
    .type(nomeAreaPromotora)

  cy.wait(1000)

  cy.get('body').then(($body) => {
    const linhaExiste = $body.find(area_promotora_localizadores.tbl_nome_teste()).length > 0
    expect(linhaExiste, `área promotora ${nomeAreaPromotora} não deveria existir após exclusão`).to.eq(false)
  })

  cy.get(area_promotora_localizadores.input_nome(), { timeout: 5000 })
    .should('be.visible')
    .clear()
})

Cypress.Commands.add('validar_campos_area_promotora', () => {
  cy.get(area_promotora_localizadores.msg_obrigatorio_area_promotora(), { timeout: 3000 })
    .should('be.visible')   
})

Cypress.Commands.add('cancelar_exclusao_area_promotora', (tipo = 'Rede Parceria') => {
  expect(tipo)
    .to.be.a('string')
    .and.not.be.empty

  const tipoTexto = tipo.trim()

  const agora = new Date()
  const dia = String(agora.getDate()).padStart(2, '0')
  const mes = String(agora.getMonth() + 1).padStart(2, '0')
  const ano = agora.getFullYear()
  const hora = String(agora.getHours()).padStart(2, '0')
  const minuto = String(agora.getMinutes()).padStart(2, '0')

  const nomeAreaPromotora = `Teste automatizado ${dia}/${mes}/${ano} ${hora}:${minuto}`
  const emailTeste = `testeautomatizado${dia}${mes}${ano}${hora}${minuto}@sme.com`

  function selecionarOpcaoAnt(inputSelector, textoOpcao) {
    const opcao = String(textoOpcao).trim()

    cy.get(inputSelector, { timeout: 8000 })
      .should('exist')
      .parents('.ant-select')
      .first()
      .find('.ant-select-selector')
      .trigger('mousedown', { which: 1, force: true })

    cy.contains('.ant-select-item-option', opcao, { timeout: 8000 })
      .should('be.visible')
      .click()
  }

  function filtrarPorNome(nome) {
    cy.get(area_promotora_localizadores.input_nome(), { timeout: 10000 })
      .should('be.visible')
      .clear()
      .type(nome)

    cy.wait(1000)
  }

  function abrirRegistroPorNome(nome) {
    cy.contains('.ant-table-row td, .ant-table-cell', nome, { timeout: 10000 })
      .should('be.visible')
      .click()
  }

  function clicarExcluir() {
    cy.get(area_promotora_localizadores.btn_exclui_area_promotora(), { timeout: 10000 })
      .should('exist')
      .should('be.visible')
      .click()

    cy.get('.ant-modal-content', { timeout: 10000 })
      .should('exist')
      .should('be.visible')
  }

  function cancelarExclusao() {
    cy.get(area_promotora_localizadores.btn_cancelar_excluir_area_promotora(), { timeout: 10000 })
      .should('exist')
      .should('be.visible')
      .click()

    cy.get('.ant-modal-content', { timeout: 10000 })
      .should('not.exist')
  }

  function confirmarExclusao() {
    cy.get(area_promotora_localizadores.btn_confirmar_modal_area_promotora(), { timeout: 10000 })
      .should('exist')
      .should('be.visible')
      .click()

    cy.get('.ant-modal-content', { timeout: 10000 })
      .should('not.exist')
  }

  cy.get(area_promotora_localizadores.btn_salvar_area_promotora(), { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.get(area_promotora_localizadores.btn_novo_nome_area_promotora(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type(nomeAreaPromotora)

  selecionarOpcaoAnt(area_promotora_localizadores.btn_novo_tipo_area_promotora(), tipoTexto)
  selecionarOpcaoAnt(area_promotora_localizadores.btn_novo_perfil_area_promotora(), 'Arquivo Histórico Municipal')

  cy.get(area_promotora_localizadores.btn_novo_telefone_area_promotora(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type('31999999999')

  cy.get(area_promotora_localizadores.btn_novo_email_area_promotora(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type(emailTeste)

  cy.contains('button', 'Salvar', { timeout: 10000 })
    .should('be.visible')
    .click()

  cy.get(area_promotora_localizadores.msg_sucesso_area_promotora(), { timeout: 10000 })
    .should('be.visible')

  filtrarPorNome(nomeAreaPromotora)
  abrirRegistroPorNome(nomeAreaPromotora)

  clicarExcluir()
  cancelarExclusao()

  cy.get(area_promotora_localizadores.btn_exclui_area_promotora(), { timeout: 10000 })
    .should('exist')
    .should('be.visible')
    .click()

  cy.get('.ant-modal-content', { timeout: 10000 })
    .should('exist')
    .should('be.visible')

  confirmarExclusao()

  cy.get(area_promotora_localizadores.input_nome(), { timeout: 15000 })
    .should('be.visible')
    .clear()
    .type(nomeAreaPromotora)

  cy.wait(1000)

  cy.contains('.ant-table-row td, .ant-table-cell', nomeAreaPromotora, { timeout: 5000 })
    .should('not.exist')
})

Cypress.Commands.add('validar_exclusao_area_promotora', () => {
  cy.get(area_promotora_localizadores.btn_salvar_area_promotora(), { timeout: 3000 })
    .should('be.visible')   
})

Cypress.Commands.add('validar_cancelamento_exclusao_area_promotora', () => {
  cy.get(area_promotora_localizadores.input_nome(), { timeout: 3000 })
    .should('be.visible')   
})

Cypress.Commands.add('tentar_excluir_area_promotora', () => {
  const nome = 'COPED - Núcleo de Formação'

  cy.get(area_promotora_localizadores.input_nome(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type(nome)

  cy.contains('.ant-table-row td, .ant-table-cell', nome, { timeout: 10000 })
    .click()

  cy.get(area_promotora_localizadores.btn_exclui_area_promotora(), { timeout: 10000 })
    .should('exist')
    .should('be.visible')
    .click()

  cy.get(area_promotora_localizadores.btn_confirmar_modal_area_promotora(), { timeout: 10000 })
    .should('exist')
    .should('be.visible')
    .click()
})

Cypress.Commands.add('validar_nao_exclusao_area_promotora', () => {
  cy.get(area_promotora_localizadores.msg_exclui_area_promotora(), { timeout: 3000 })
    .should('be.visible')   
})

Cypress.Commands.add('editar_area_promotora', () => {
  const agora = new Date()
  const dia = String(agora.getDate()).padStart(2, '0')
  const mes = String(agora.getMonth() + 1).padStart(2, '0')
  const ano = agora.getFullYear()
  const hora = String(agora.getHours()).padStart(2, '0')
  const minuto = String(agora.getMinutes()).padStart(2, '0')

  const emailTeste = `testeautomatizado${dia}${mes}${ano}${hora}${minuto}@sme.com`

  function selecionarOpcaoAnt(inputSelector, textoOpcao) {
    const opcao = String(textoOpcao).trim()

    cy.get(inputSelector, { timeout: 8000 })
      .should('exist')
      .parents('.ant-select')
      .first()
      .find('.ant-select-selector')
      .trigger('mousedown', { which: 1, force: true })

    cy.contains('.ant-select-item-option', opcao, { timeout: 8000 })
      .should('be.visible')
      .click()
  }

  cy.get(area_promotora_localizadores.btn_salvar_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .click()

  cy.get(area_promotora_localizadores.btn_novo_nome_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .clear()
    .type('Teste automatizado')

  selecionarOpcaoAnt(
    area_promotora_localizadores.btn_novo_tipo_area_promotora(),
    'Rede Direta'
  )

  selecionarOpcaoAnt(
    area_promotora_localizadores.btn_novo_perfil_area_promotora(),
    'Arquivo Histórico Municipal'
  )

  cy.get(area_promotora_localizadores.btn_novo_telefone_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .clear()
    .type('31999999999')

  cy.get(area_promotora_localizadores.btn_novo_email_area_promotora(), { timeout: 5000 })
    .should('be.visible')
    .clear()
    .type(emailTeste)

  cy.contains('button', 'Salvar', { timeout: 5000 })
    .should('be.visible')
    .click()

  cy.get(area_promotora_localizadores.msg_sucesso_area_promotora(), { timeout: 5000 })
    .should('be.visible')

  cy.url({ timeout: 10000 }).should('include', '/cadastro/area-promotora')

  cy.get(area_promotora_localizadores.input_nome(), { timeout: 10000 })
    .should('be.visible')
    .clear()
    .type('Teste automatizado')

  cy.get(area_promotora_localizadores.tbl_nome(), { timeout: 10000 })
    .should('exist')
    .should('be.visible')
    .first()
    .click()
  
  cy.url({ timeout: 10000 }).should('include', '/cadastro/area-promotora/editar')

  cy.get(area_promotora_localizadores.btn_edita_area_promotora(), { timeout: 30000 })
    .should('be.visible')
    .click()  

  cy.get(area_promotora_localizadores.input_digite_nome(), { timeout: 30000 })
    .should('be.visible')
    .clear()
    .type('Teste automatizado')

  cy.get('body').then(($body) => {
    const linhaExiste = $body.find(area_promotora_localizadores.tbl_nome_teste()).length > 0

    if (linhaExiste) {
      cy.get(area_promotora_localizadores.tbl_nome_teste(), { timeout: 5000 })
        .should('be.visible')
        .click()

      cy.get(area_promotora_localizadores.btn_exclui_area_promotora(), { timeout: 15000 })
        .should('be.visible')
        .click()

      cy.get(area_promotora_localizadores.btn_confirmar_modal_area_promotora(), { timeout: 5000 })
        .should('be.visible')
        .click()
    }
  })
})