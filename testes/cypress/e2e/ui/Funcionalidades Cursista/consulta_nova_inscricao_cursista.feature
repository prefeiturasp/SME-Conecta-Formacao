# language: pt

Funcionalidade: Consulta de Nova inscrição

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Cursista"  

  Esquema do Cenário: Validar a consulta de nova incrição: <caso>
    Quando acesso o menu Minhas Inscrições
    E clico em explorar formações
    Quando exibe para consulta de novas formações
    Então mostra o "<campo>" no filtro "<valor>" de nova inscrição disponíveis
    
    Exemplos:
      | campo   | valor          | caso                      |
      | público | AGENTE ESCOLAR | Selecionar público alvo   |
      | título  | Teste          | Buscar título             |
      | área    | Teste          | Selecionar área promotora |
      | data    | 01/01/2026     | Filtrar data              |
      | formato | Presencial     | Selecionar formato        |
      | palavra |  DF            | Selecionar palavra chave  |

  Esquema do Cenário: Validar a consulta de: <caso>
    Quando acesso o menu Minhas Inscrições
    E clico em explorar formações
    Quando exibe para consulta de novas formações
    Então carrega o "<campo>" das próximas formações disponíveis

    Exemplos:
      | campo | caso               |
      | card  | Próximas formações |

  Esquema do Cenário: Validar a consulta de: <caso>
    Quando acesso o menu Minhas Inscrições
    E clico em explorar formações
    Quando exibe para consulta de novas formações
    Então carrega os detalhes "<campo>" das próximas formações disponíveis

    Exemplos:
      | campo | caso                 |
      | card  | Detalhes da formação |
