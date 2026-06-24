class Meus_Certificados_Localizadores {

  // consulta
  menu_meus_certificados = () => ':nth-child(2) > .ant-menu-submenu-title'
  opcao_meus_certificados = () => 'li.ant-menu-item'
  btn_filtrar = () => ':nth-child(2) > .ant-btn'
  btn_limpar = () => '.ant-row-end > :nth-child(1) > .ant-btn'
  campo_homologacao = () => '#INPUT_NUMERO' 
  campo_nome = () => '#INPUT_TEXTO'
  campo_emissao = () => '#dataEmissao'
  campo_codigo = () => '#INPUT_NUMERO'
  select_tipo = () => '#tipoCertificado'  
  tbl_meus_certificados = () => '.ant-table-thead > tr > :nth-child(1)' 

}

export default Meus_Certificados_Localizadores 