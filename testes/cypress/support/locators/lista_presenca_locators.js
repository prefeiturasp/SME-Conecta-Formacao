class Lista_Presenca_Localizadores {

  // consulta
  menu_formacoes = () => ':nth-child(3) > .ant-menu-submenu-title'
  menu_lista_presenca = () => '.ant-menu-title-content'
  btn_filtrar = () => '.ant-row-end > :nth-child(2) > .ant-btn'
  btn_limpar = () => '.ant-row-end > :nth-child(1) > .ant-btn'
  campo_nome = () => '#CF_INPUT_NOME_FORMACAO'
  select_area = () => '#CF_SELECT_AREA_PROMOTORA'
  campo_codigo = () => '#CF_INPUT_CODIGO_FORMACAO'
  campo_homologacao = () => '#CF_INPUT_NUMERO_HOMOLOGACAO'   
  campo_envio = () => '#dataEnvio'  
  select_situacao = () => '#situacao'  
  tbl_lista_presenca = () => '.ant-table-thead > tr > :nth-child(1)' 
  btn_acoes = () => 'button.ant-dropdown-trigger'
  btn_gerar_arquivo = () => 'span'
  msg_sucesso = () => '.ant-notification-notice-message'

}

export default Lista_Presenca_Localizadores 