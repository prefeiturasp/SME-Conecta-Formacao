# language: pt

Funcionalidade: Lista presença CODAF

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"

  Esquema do Cenário: Gerar TXT EOL
    Quando acesso o menu Lista Presença Codaf
    E filtro a presença nas formações "<situacao>"
    Então o sistema permite baixar o TXT CODAF
      
    Exemplos:
      | situacao      |
      | Aguardando DF | 

  Esquema do Cenário: Baixar relatório CODAF
    Quando acesso o menu Lista Presença Codaf
    E filtro a presença nas formações "<situacao>"
    Então o sistema permite baixar o relatório CODAF
         
    Exemplos:
      | situacao   |
      | Finalizado | 

  Esquema do Cenário: Filtrar por: <caso>
    Quando acesso o menu Lista Presença Codaf
    E preencho o campo "<opcao>" com "<valor>" na presença das formações
    Então busca na Lista Presença Codaf com "<campo>"

    Exemplos:
      | campo                 | opcao    | valor      | caso                           |
      | Nome da formação      | nome     | Teste      | Nome da formação               |
      | Área promotora        | área     | Teste      | Área promotora                 |
      | Código da formação    | código   | 123        | Código da formação             |
      | Número de homologação | número   | 572        | Número de homologação          |
      | Data de finalização   | data     | 01/01/2026 | Data de envio para finalização | 

  Esquema do Cenário: Filtrar a situação: <caso>
    Quando acesso o menu Lista Presença Codaf
    E preencho o campo "<opcao>" com "<valor>" na presença das formações
    Então busca na Lista Presença Codaf com "<campo>"

    Exemplos:
      | campo                 | opcao    | valor      | caso              |
      | Situação              | situação | Iniciado   | Iniciado          |
      | Situação              | situação | Iniciado   | Aguardando DF     |
      | Situação              | situação | Iniciado   | Devolvido pelo DF |
      | Situação              | situação | Finalizado | Finalizado        |

  Esquema do Cenário: Registros não encontrados para os filtros aplicados
    Quando acesso o menu Lista Presença Codaf
    E filtro dado não existente na presença nas formações
    Então o sistema informa dados não encontrados ao baixar o relatório CODAF

  Esquema do Cenário: Limpar filtros de lista de presença
    Quando acesso o menu Lista Presença Codaf
    E removo os filtros na Lista Presença Codaf
    Então limpa na presença nas formações

