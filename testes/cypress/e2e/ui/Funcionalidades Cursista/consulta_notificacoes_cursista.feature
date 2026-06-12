# language: pt

Funcionalidade: Consulta de Notificações

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Cursista"

  Esquema do Cenário: Visualizar novas notificações
    Quando acesso o menu Notificações
     Então visualizo as novas notificações na listagem

  Esquema do Cenário: Filtrar notificações por: <caso>
    Quando acesso o menu Notificações
    E preencho o campo "<tipo>" com "<valor>" nas notificações
    Então busca na listagem em notificações com "<campo>"

    Exemplos:
      | campo     | tipo      | valor     | caso                |
      | código    | código    | 34        | Código              |
      | tipo      | tipo      | Proposta  | Tipo - Proposta     |
      | tipo      | tipo      | Codaf     | Tipo - Codaf        |
      | tipo      | tipo      | Relatório | Tipo - Relatório    |
      | categoria | categoria | Alerta    | Categoria - Alerta  |
      | categoria | categoria | Ação      | Categoria - Ação    |
      | categoria | categoria | Aviso     | Categoria - Aviso   |
      | categoria | categoria | Informe   | Categoria - Informe |
      | título    | título    | Teste     | Título              |
      | situação  | situação  | Não lida  | Não lida            |
      | situação  | situação  | Lida      | Lida                |
