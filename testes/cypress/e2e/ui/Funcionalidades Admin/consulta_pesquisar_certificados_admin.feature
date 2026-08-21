# language: pt

Funcionalidade: Pesquisar certificados

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"

  Esquema do Cenário: Filtrar certificados
    Quando acesso o menu Pesquisar certificados
    E filtro todos certificados na pesquisa
    Então o sistema exibe todos os dados de certificados

  Esquema do Cenário: Baixar certificado
    Quando acesso o menu Pesquisar certificados
    E filtro todos certificados na pesquisa
    E clico para baixar o certificado selecionado
    Então o sistema realiza o download dos certificados

  Esquema do Cenário: Baixar certificados em lote
    Quando acesso o menu Pesquisar certificados
    E filtro todos certificados na pesquisa
    E clico para baixar todos certificados selecionados
    Então o sistema realiza o download dos certificados

  Esquema do Cenário: Dados não encontrados
    Quando acesso o menu Pesquisar certificados
    E filtro dado de certificado inexistente na pesquisa
    Então o sistema sem dados de certificados

  Esquema do Cenário: Filtrar por: <caso>
    Quando acesso o menu Pesquisar certificados
    E filtro o campo "<opcao>" de certificado com "<valor>" na pesquisa  
    Então busca nas pequisa de certificado com "<campo>"

    Exemplos:
      | campo                 | opcao       | valor              | caso                           |
      | Nome da formação      | nome        | Nome               | Nome da formação               |
      | Tipo de certificado   | tipo        | Cursista           | Tipo de certificado            |
      | Código da formação    | código      | 1                  | Código da formação             |
      | Número de homologação | número      | 1                  | Número de homologação          |
      | Código do certificado | certificado | 1                  | Código do certificado          |
      | RF ou CPF do cursista | documento   | REGISTRO_FUNCIONAL | RF ou CPF do cursista          |
      | RF do regente         | regente     | REGISTRO_FUNCIONAL | RF do regente                  |
      | Nome do cursista      | cursista    | Nome               | Nome do cursista               |     
      | Data de finalização   | data        | 01/01/2026         | Data de envio para finalização |
      | DRE                   | diretoria   | TODAS              | DRE                            | 