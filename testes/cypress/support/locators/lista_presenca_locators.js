class Lista_Presenca_Localizadores {

  // cadastro
  btn_novo_registro = () => '#CF_BUTTON_NOVO'
  btn_continuar_registro = () => '.ant-modal-footer > .ant-btn-primary'
  btn_salvar = () => '#CF_BUTTON_SALVAR'
  btn_cancelar = () => '#CF_BUTTON_CANCELAR'
  btn_voltar = () => '#CF_BUTTON_VOLTAR'
  campo_turma = () => '#turmaId'
  opcao_turma = () => '.ant-select-dropdown:not(.ant-select-dropdown-hidden) .ant-select-item-option-content'

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
  opcao_situacao = () => '.ant-select-item-option-content'
  tbl_lista_presenca = () => '.ant-table-thead > tr > :nth-child(1)'
  btn_acoes = () => '.ant-dropdown-trigger'
  btn_gerar_arquivo = () => 'span'
  msg_sucesso = () => '.ant-notification-notice-message'

  // exclusão
  tbl_lista_presenca = () => '.ant-table-row > :nth-child(1)'
  btn_excluir = () => '#CF_BUTTON_EXCLUIR'
  btn_confirmar_exclusao = () => '.ant-modal-footer > .ant-btn-primary'
}

export default Lista_Presenca_Localizadores