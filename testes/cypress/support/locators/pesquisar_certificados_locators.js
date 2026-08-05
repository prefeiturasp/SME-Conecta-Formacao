class Pesquisar_Certificados_Localizadores {

  // consulta
  menu_formacoes = () => ':nth-child(3) > .ant-menu-submenu-title'
  menu_lista_presenca = () => '.ant-menu-title-content'
  btn_filtrar = () => '.ant-row-end > .ant-col > .ant-btn'
  campo_nome_formacao = () => '#CF_INPUT_NOME_FORMACAO'
  campo_nome = () => '#CF_INPUT_NOME_FORMACAO'
  select_tipo = () => '#CF_SELECT_TIPO_FORMACAO'
  campo_codigo = () => '#CF_INPUT_CODIGO_FORMACAO'
  campo_numero = () => '#CF_INPUT_CODIGO_HOMOLOGACAO'
  campo_certificado = () => '#CF_INPUT_CERTIFICADO'
  campo_documento = () => '#CF_INPUT_DOCUMENTO'
  campo_regente = () => '#CF_INPUT_REGENTE'
  campo_cursista = () => '#CF_INPUT_CURSISTA'
  campo_emissao = () => '#dataEnvio'  
  select_dre = () => '#situacao' 
  tbl_certificados = () => 'tr > [title="Nome da formação"]' 
  check = () => ':nth-child(2) > .ant-table-selection-column > .ant-checkbox-wrapper > .ant-checkbox > .ant-checkbox-input'
  check_todos = () => '.ant-table-thead > tr > .ant-table-selection-column > .ant-checkbox-wrapper > .ant-checkbox > .ant-checkbox-input'
  btn_baixar_certificado = () => 'span'
  msg_sucesso = () => '.ant-notification-notice-description'

}

export default Pesquisar_Certificados_Localizadores 