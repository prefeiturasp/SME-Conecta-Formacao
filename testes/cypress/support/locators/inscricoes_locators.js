class Inscricoes_Localizadores {

  // consulta
  input_codigo = () => '#CodigoFormacao'
  input_nome = () => '#NomeFormacao'
  select_data = () => '#DataInscricao'
  input_turma = () => ':nth-child(4) > .ant-form-item > .ant-row > .ant-form-item-control > .ant-form-item-control-input'
  input_turma_digitavel = () => `${this.input_turma()} input`
  input_periodo_inicial = () => '#periodo'
  input_periodo_final = () => ':nth-child(3) > input'
  input_situacao = () => '#Situacao'
  ant_select_selector = () => '.ant-select-selector'
  ant_select_opcoes_visiveis = () => '.ant-select-dropdown:visible .ant-select-item-option'
  tbl_finalizadas = () => '#rc-tabs-0-tab-finalizadas'
  input_nome_finalizada = () => '#rc-tabs-0-panel-finalizadas > .ant-form > [style="margin-left: -8px; margin-right: -8px; row-gap: 16px;"] > :nth-child(1) > .ant-form-item > .ant-row > .ant-form-item-control > .ant-form-item-control-input > .ant-form-item-control-input-content > .ant-input-affix-wrapper > #NomeFormacao'
  input_situacao_finalizada = () => '#rc-tabs-0-panel-finalizadas #SituacaoInscricao'
  input_periodo_inicial_finalizada = () => '#rc-tabs-0-panel-finalizadas > .ant-form > [style="margin-left: -8px; margin-right: -8px; row-gap: 16px;"] > :nth-child(3) > .ant-form-item > .ant-row > .ant-form-item-control > .ant-form-item-control-input > .ant-form-item-control-input-content > .ant-picker > :nth-child(1) > #periodo'
  input_periodo_final_finalizada = () => '#rc-tabs-0-panel-finalizadas > .ant-form > [style="margin-left: -8px; margin-right: -8px; row-gap: 16px;"] > :nth-child(3) > .ant-form-item > .ant-row > .ant-form-item-control > .ant-form-item-control-input > .ant-form-item-control-input-content > .ant-picker > :nth-child(1) > #periodo'
  btn_explorar_formacoes = () => '#CF_BUTTON_NOVO'

  // editar  
  btn_modal_alterar = () => '#CF_BUTTON_MODAL_ALTERAR'
  btn_modal_cancelar = () => '#CF_BUTTON_MODAL_CANCELAR'
  
  input_senha_atual = () => '#CF_INPUT_SENHA_ATUAL'
  input_nova_senha = () => ':nth-child(2) > .ant-form-item > .ant-row > .ant-form-item-control > .ant-form-item-control-input > .ant-form-item-control-input-content > .ant-input-affix-wrapper > #CF_INPUT_SENHA'
  input_confirmacao_senha = () => '#CF_INPUT_CONFIRMAR_SENHA'
  msgm_alteracao_sucesso = () => '.ant-notification-notice-description'  

}

export default Inscricoes_Localizadores 