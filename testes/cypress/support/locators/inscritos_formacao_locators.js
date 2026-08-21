class Inscritos_Formacao_Localizadores {

  // consulta
  submenu_relatorios_inscritos_formacao = () => '.ant-menu-submenu-title'
  item_menu_relatorios_inscritos_formacao = () => 'li.ant-menu-item .ant-menu-title-content'
  input_codigo_formacao = () => '#INPUT_NUMERO'
  input_codigo_homologacao = () => '#numeroHomologacao'
  select_turma = () => '#propostaTurmaId'
  select_formato = () => '#formato'
  input_nome = () => '#INPUT_TEXTO' 
  select_area_promotora = () => '#CF_SELECT_AREA_PROMOTORA'
  select_situacao = () => '#situacaoInscricao'
  select_opcao = () => '.ant-select-item-option-content'
  select_inicio = () => '#periodoRealizacao'
  select_fim = () => ':nth-child(3) > input'
  messagem = () => '.ant-notification-notice-message'
}

export default Inscritos_Formacao_Localizadores 