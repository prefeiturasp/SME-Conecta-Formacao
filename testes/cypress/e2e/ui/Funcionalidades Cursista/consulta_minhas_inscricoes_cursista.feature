# language: pt

Funcionalidade: Consulta de Minhas Inscrições

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Cursista"

  Esquema do Cenário: Preencher filtros de Minhas Inscrições em andamento: <caso>
    Quando acesso o menu Minhas Inscrições
    E preencho o campo "<tipo>" com "<valor>" nas inscriçõees ativas
    Então exibe os campos de Minhas Inscrições em andamento "<tipo>"

    Exemplos:
      | tipo     | valor         | caso               |
      | codigo   | 1             | Código da formação |
      | nome     | teste         | Nome da formação   |
      | data     | 01/01/2026    | Data da inscrição  |
      | turma    | Turma 1       | Turma              |
      | situacao | Confirmada    | Situação           |

  Esquema do Cenário: Consultar intervalo de período em Minhas Inscrições
    Quando acesso o menu Minhas Inscrições
    Então retorna o período com "01/01/2026" e "31/12/2026" em Minhas Inscrições
  
  Esquema do Cenário: Validar filtros formação em andamento: <caso>
    Quando acesso o menu Minhas Inscrições
    Então exibe os campos de Minhas Inscrições em andamento "<tipo>"

    Exemplos:
      | tipo     | caso                              |
      | codigo   | Código da formação                |
      | nome     | Nome da formação                  |
      | data     | Data da inscrição                 |
      | turma    | Turma                             |
      | periodo  | Período de realização da formação |
      | situacao | Situação                          |

  Esquema do Cenário: Preencher filtros de Minhas Inscrições finalizadas: <caso>
    Quando acesso o menu Minhas Inscrições
    E preencho o campo "<tipo>" com "<valor>" nas formações concluídas
    Então exibe os campos de Minhas Inscrições finalizadas "<tipo>"

    Exemplos:
      | tipo     | valor        | caso             |
      | nome     | teste        | Nome da formação |
      | situacao | Confirmada   | Situação         |

  Esquema do Cenário: Consultar intervalo de período em Minhas Inscrições finalizadas
    Quando acesso o menu Minhas Inscrições
    Então preencho o período com "01/01/2026" e "31/12/2026" nas Inscrições finalizadas

  Esquema do Cenário: Validar filtros formação finalizada: <caso>
    Quando acesso o menu Minhas Inscrições
    Então exibe os campos de Minhas Inscrições em finalizadas "<tipo>"

    Exemplos:
      | tipo    | caso                              |
      | nome    | Nome da formação                  |
      | periodo | Período de realização da formação |

  Esquema do Cenário: Validar a consulta de novas formações
    Quando acesso o menu Minhas Inscrições
    E clico em explorar formações
    Então exibe para consulta de novas formações