class Inscricoes_Localizadores {

  // consulta
  menu_inscricoes = () => 'span.ant-menu-title-content'
  campo_codigo = () => '#CF_INPUT_CODIGO_FORMACAO'
  campo_nome = () => '#CF_INPUT_NOME_FORMACAO'
  campo_homologacao = () => '#CF_INPUT_NUMERO_HOMOLOGACAO'
  listagem_cursos = () => '[data-row-key="344"] > :nth-child(2)'
  abrir_turma = () => '.ant-select-selection-overflow'
  selecionar_turma = () => '.ant-select-tree-checkbox-inner'
  selecionar_cargo = () => '#CF_SELECT_CARGO_FUNCAO'
  selecionar_situacao = () => '#CF_SELECT_SITUACAO_INSCRICAO'
  campo_registro = () => '#CF_INPUT_RF'
  campo_documento = () => 'input[placeholder="CPF"]'
  campo_nome_cursista = () => '#CF_INPUT_NOME' 
  
  // cadastro
  btn_nova_inscricao = () => '#CF_BUTTON_NOVO'
  selecionar_turma_cronograma = () => '#CF_SELECT_TURMA_CRONOGRAMA'
  selecionar_turma_1 = () => '.ant-select-item-option-content'
  campo_rf= () => '#INPUT_RF'
  buscar_rf = () => '.ant-input-wrapper > .ant-input-group-addon > .ant-btn'
  salvar_inscricao = () => '#CF_BUTTON_NOVO'
  selecionar_cargo_inscricao = () => 'input#CF_SELECT_CARGO.ant-select-selection-search-input'
  msg_inscricao = () => '.ant-notification-notice-description'
  cancelar_inscricao = () => '[style="text-align: center;"] > .ant-row > :nth-child(1) > .ant-btn'
  reativar_inscricao = () => '[style="text-align: center;"] > .ant-row > :nth-child(2) > .ant-btn'
  esperar_inscricao = () => '[style="text-align: center;"] button.ant-btn-icon-only'
  confirmar_inscricao = () => '[style="text-align: center;"] button.ant-btn-icon-only'
  confirmar_modal = () => '.ant-modal-confirm-btns > .ant-btn-default'
  motivo_cancelamento = () => '#CF_INPUT_AREA_TEXTO'
  botao_cancelar_inscricao = () => '.ant-modal-footer > .ant-btn-default'
  msg_obrigatorio = () => '.ant-form-item-explain-error'

}

export default Inscricoes_Localizadores 