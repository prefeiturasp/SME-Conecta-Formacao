# language: pt

Funcionalidade: Cadastro de Nova inscrição

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Cursista"  

  Esquema do Cenário: Validar cadastrar: <caso>
    Quando acesso o menu Minhas Inscrições
    E clico em explorar formações
    Quando exibe para consulta de novas formações
    Então clico em "<campo>" nova inscrição disponível

    Exemplos:   
      | campo  | caso                            |
      | enviar | Inscrição realizada com sucesso |

  Esquema do Cenário: Validar cancelar: <caso>
    Quando acesso o menu Minhas Inscrições
    E clico em explorar formações
    Quando exibe para consulta de novas formações
    Então retorna no "<campo>" nova inscrição disponível

    Exemplos:
      | campo    | caso                     |
      | cancelar | Inscrição não cadastrada |

  Esquema do Cenário: Validar campo obrigatório: <caso>
    Quando acesso o menu Minhas Inscrições
    E clico em explorar formações
    Quando exibe para consulta de novas formações
    Então retorna que "<campo>" é obrigatório em nova inscrição disponível

    Exemplos:
      | campo       | caso                   |
      | turma       | Selecionar uma Turma   |
      | deficiência | Pessoa com Deficiência |
