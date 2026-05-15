class Inscricoes_Localizadores {

  // consultar
  select_publico_alvo = () => ':nth-child(1) > .ant-form-item > .ant-row > .ant-form-item-control > .ant-form-item-control-input > .ant-form-item-control-input-content > .ant-select > .ant-select-selector'
  input_titulo = () => '#INPUT_TEXTO'
  select_area_promotora = () => ':nth-child(1) > :nth-child(3) > .ant-form-item > .ant-row > .ant-form-item-control > .ant-form-item-control-input > .ant-form-item-control-input-content > .ant-select > .ant-select-selector > .ant-select-selection-overflow'
  select_data = () => '#rangerPicker' 
  select_formato = () => ':nth-child(2) > .ant-form-item > .ant-row > .ant-form-item-control > .ant-form-item-control-input > .ant-form-item-control-input-content > .ant-select > .ant-select-selector > .ant-select-selection-overflow'
  select_palavras_chave = () => ':nth-child(2) > :nth-child(3) > .ant-form-item > .ant-row > .ant-form-item-control > .ant-form-item-control-input > .ant-form-item-control-input-content > .ant-select > .ant-select-selector > .ant-select-selection-overflow'
  btn_buscar_formacoes = () => '.ant-row > .ant-btn'  
  btn_detalhes_formacoes = () => 'button.ant-btn-primary:contains("Saiba mais")'
  card_proximas_formacoes = () => '.ant-card > .ant-card-body'

  // cadastrar
  btn_enviar_inscricao = () => '.ant-col > .ant-btn'
  btn_voltar = () => '#CF_BUTTON_VOLTAR'
  input_turma = () => '#CF_SELECT_TURMA_INSCRICAO'
  input_deficiencia = () => '#pessoaComDeficiencia'  
  btn_nova_inscricao = () => '#CF_BUTTON_NOVO > span'
  btn_salvar_informacoes = () => '.ant-modal-footer > .ant-btn-primary'
  btn_nao_enviar_informacoes = () => '.ant-modal-confirm-btns > .ant-btn-text > span'
}

export default Inscricoes_Localizadores 