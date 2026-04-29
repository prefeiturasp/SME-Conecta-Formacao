class Area_Promotora_Localizadores {

  // consulta
  menu_area_promotora = () => 'span.ant-menu-title-content'
  titulo_area_promotora = () => 'h1, h2, h3, .page-title'
  input_nome = () => '.ant-input'
  select_tipo = () => '.ant-select .ant-select-selector'  
  lista_opcoes_tipo = () => '.ant-select-item-option'
  tbl_linhas = () => '.ant-table-tbody > tr:not(.ant-table-placeholder)'
  tbl_nome = () => '.ant-table-tbody > tr > :nth-child(1)'
  tbl_nome_teste = () => '.ant-table-row > :nth-child(1)'
  tbl_tipo = () => '.ant-table-tbody > tr > :nth-child(2)'

  // cadastro
  btn_novo_area_promotora = () => '#CF_BUTTON_NOVO > span'
  btn_novo_nome_area_promotora = () => '#CF_INPUT_NOME'
  btn_novo_tipo_area_promotora = () => '#tipo'
  btn_novo_perfil_area_promotora = () => '#perfil'
  btn_novo_telefone_area_promotora = () => '#CF_INPUT_TELEFONE_1'
  btn_novo_email_area_promotora = () => '#CF_INPUT_EMAIL_1'
  btn_novo_salvar_area_promotora = () => '#CF_BUTTON_NOVO > span'
  btn_novo_cancelar_area_promotora = () => '#CF_BUTTON_CANCELAR > span'
  msg_obrigatorio_area_promotora = () => '.ant-form-item-explain-error'
  msg_sucesso_area_promotora = () => '.ant-notification-notice-description'

  // editar
  btn_edita_area_promotora = () => '#CF_BUTTON_NOVO'
  btn_confirmar_modal_area_promotora = () => '.ant-modal-confirm-btns > .ant-btn-default'

  // excluir
  btn_exclui_area_promotora = () => '#CF_BUTTON_EXCLUIR'
  btn_cancelar_excluir_area_promotora = () => '.ant-modal-confirm-btns > .ant-btn-text > span'
  msg_exclui_area_promotora = () => '.ant-notification-notice-message'
}

export default Area_Promotora_Localizadores