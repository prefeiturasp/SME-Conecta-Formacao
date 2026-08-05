class Rede_Parceria_Localizadores {

  // criar e consultar
  menu_rede_parceria = () => 'li.ant-menu-item'
  btn_novo = () => '#CF_BUTTON_NOVO'
  input_cpf = () => '#cpf'
  input_nome_usuario = () => '#CF_INPUT_NOME'  
  input_email = () => '#INPUT_EMAIL'
  input_telefone = () => '#telefone' 
  select_situacao = () => '#CF_SELECT_SITUACAO'
  btn_salvar = () => '#CF_BUTTON_SALVAR'
  btn_confirmar_cadastro_usuario = () => '.ant-modal-confirm-btns > .ant-btn-default'
  msg_campo_obrigatorio = () => '#cpf_help > .ant-form-item-explain-error'
  tbl_cpf = () => '.ant-table-row > :nth-child(3)'
  filtro_area_promotora = () => '#CF_SELECT_AREA_PROMOTORA'
  opcoes_area_promotora = () => '.ant-select-item-option-content'
  filtro_nome = () => '#CF_INPUT_NOME_FORMACAO'

  // editar
  btn_voltar = () => '#CF_BUTTON_VOLTAR'
   
  // excluir
  btn_excluir_usuario = () => '#CF_BUTTON_EXCLUIR'
  btn_cancelar_exclusao_usuario = () => '.ant-modal-confirm-btns > .ant-btn-text > span'

}

export default Rede_Parceria_Localizadores 