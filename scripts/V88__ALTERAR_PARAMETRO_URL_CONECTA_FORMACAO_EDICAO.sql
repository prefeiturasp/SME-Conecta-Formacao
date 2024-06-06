--> Alterar o nome do parâmetro         
update parametro_sistema 
set nome = 'UrlConectaFormacaoEdicaoProposta',
valor = 'https://hom-conectaformacao.sme.prefeitura.sp.gov.br/cadastro/cadastro-de-propostas/editar/{0}'
where tipo = 8;

