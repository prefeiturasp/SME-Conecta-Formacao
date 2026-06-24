# language: pt

Funcionalidade: Meus certificados

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Cursista"

  Esquema do Cenário: Baixar certificado de conclusão
    Quando acesso o menu Meus Certificados
    E filtro certificados obtidos nas formações
    Então o sistema permite baixar certificado de conclusão

  Esquema do Cenário: Filtrar meus certificados por: <caso>
    Quando acesso o menu Meus Certificados
    E preencho o campo "<opcao>" com "<valor>" nos certificados
    Então busca na listagem de Meus Certificados com "<campo>"

    Exemplos:
      | campo       | opcao  | valor      | caso                  |
      | homologação | código | 123        | Código de homologação |
      | formação    | nome   | Teste      | Nome da formação      |
      | emissão     | data   | 01/01/2026 | Data de emissão       |
      | código      | número | 572        | Código do certificado |
      | tipo        | tipo   | Cursista   | Tipo de certificado   |

  Esquema do Cenário: Limpar filtros em meus certificados
    Quando acesso o menu Meus Certificados
    E removo os filtros nos certificados obtidos nas formações
    Então limpa os filtros em meus certificados

