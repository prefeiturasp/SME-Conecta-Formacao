class Pesquisar_Certificados_Localizadores {

  // consulta
  menu_formacoes = () => ':nth-child(3) > .ant-menu-submenu-title'
  menu_lista_presenca = () => '.ant-menu-title-content'
  btn_filtrar = () => '.ant-row-end > .ant-col > .ant-btn'
  campo_nome_formacao = () => '#CF_INPUT_NOME_FORMACAO'
  campo_nome = () => '#CF_INPUT_NOME_FORMACAO'
  select_tipo = () => '#tipoCertificado'
  select_opcao = () => '.ant-select-item-option-content'
  campo_codigo = () => '#CF_INPUT_CODIGO_FORMACAO'
  campo_numero = () => '#CF_INPUT_NUMERO_HOMOLOGACAO'
  campo_certificado = () => '#INPUT_NUMERO'
  campo_documento = () => '#CF_INPUT_RF'
  campo_regente = () => ':nth-child(3) > .ant-form-item > .ant-row > .ant-form-item-control > .ant-form-item-control-input > .ant-form-item-control-input-content > .ant-input-affix-wrapper > #INPUT_TEXTO'
  campo_cursista = () => ':nth-child(1) > .ant-form-item > .ant-row > .ant-form-item-control > .ant-form-item-control-input > .ant-form-item-control-input-content > .ant-input-affix-wrapper > #INPUT_TEXTO'
  campo_emissao = () => '#dataEmissao'  
  select_dre = () => '#CF_SELECT_DRE' 
  tbl_certificados = () => 'tr > [title="Nome da formação"]' 
  check = () => ':nth-child(2) > .ant-table-selection-column > .ant-checkbox-wrapper > .ant-checkbox > .ant-checkbox-input'
  check_todos = () => '.ant-table-thead > tr > .ant-table-selection-column > .ant-checkbox-wrapper > .ant-checkbox > .ant-checkbox-input'
  btn_baixar_certificado = () => 'span'
  msg_sucesso = () => '.ant-notification-notice-description'

}

export default Pesquisar_Certificados_Localizadores 