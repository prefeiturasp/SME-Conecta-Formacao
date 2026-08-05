class Meus_Dados_Localizadores {

  // consulta
  submenu_meus_dados = () => '.ant-menu-submenu-title'
  item_menu_meus_dados = () => 'li.ant-menu-item .ant-menu-title-content'
  input_nome = () => '#CF_INPUT_NOME'
  input_email = () => '#CF_INPUT_EMAIL'
  select_tipo = () => '.ant-row-no-wrap > .ant-form-item > .ant-row > .ant-form-item-control > .ant-form-item-control-input > .ant-form-item-control-input-content > .ant-select > .ant-select-selector > .ant-select-selection-item'
  input_email_educacional = () => '#CF_INPUT_EMAIL'
  input_senha = () => '#CF_INPUT_SENHA' 
  select_pessoa_deficiencia = () => ':nth-child(10) > p'

  // editar
  btn_alterar_nome = () => ':nth-child(3) > .ant-row-no-wrap > .ant-btn'
  btn_alterar_email = () => ':nth-child(4) > .ant-row-no-wrap > .ant-btn'
  btn_alterar_senha = () => ':nth-child(9) > .ant-row-no-wrap > .ant-btn' 
  btn_salvar = () => '#CF_BUTTON_SALVAR'
  
  btn_modal_alterar = () => '#CF_BUTTON_MODAL_ALTERAR'
  btn_modal_cancelar = () => '#CF_BUTTON_MODAL_CANCELAR'
  
  input_senha_atual = () => '#CF_INPUT_SENHA_ATUAL'
  input_nova_senha = () => ':nth-child(2) > .ant-form-item > .ant-row > .ant-form-item-control > .ant-form-item-control-input > .ant-form-item-control-input-content > .ant-input-affix-wrapper > #CF_INPUT_SENHA'
  input_confirmacao_senha = () => '#CF_INPUT_CONFIRMAR_SENHA'
  msgm_alteracao_sucesso = () => '.ant-notification-notice-description'  

}

export default Meus_Dados_Localizadores 