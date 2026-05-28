# language: pt

Funcionalidade: Cadastro de Inscrições

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"

  Esquema do Cenário: Validar nova inscrição: <caso>
    Quando acesso o menu Inscrições
    E seleciono a formação em Inscrições
    E clico no botão de nova inscrição
    Então realiza a inscrição manual

    Exemplos:
      | tipo   | valor | caso                  |
      | código | 344   | Realizada com sucesso |

  Esquema do Cenário: Validar inscrição: <caso>
    Quando acesso o menu Inscrições
    E seleciono a formação em Inscrições
    E clico no botão de nova inscrição
    Então informa que o cursista já está matriculado

    Exemplos:
      | tipo   | valor | caso                                 |
      | código | 344   | Não permitir cursista já matriculado |

  Esquema do Cenário: Validar inscrição: <caso>
    Quando acesso o menu Inscrições
    E seleciono a formação em Inscrições
    E clico no botão de nova inscrição com usuário inexistente
    Então informa que o cursista é inválido

    Exemplos:
      | tipo   | valor | caso                    |
      | código | 344   | Cursista não encontrado |

  Esquema do Cenário: Validar inscrição alterada: <caso>
    Quando acesso o menu Inscrições
    E seleciono a formação em Inscrições
    E clico no botão de espera na inscrição
    Então realiza a espera da inscrição manual

    Exemplos:
      | tipo   | valor | caso      |
      | código | 344   | Em espera |

  Esquema do Cenário: Validar inscrição alterada: <caso>
    Quando acesso o menu Inscrições
    E seleciono a formação em Inscrições
    E clico no botão de confirmar inscrição
    Então realiza a confirmação da inscrição manual

    Exemplos:
      | tipo   | valor | caso       |
      | código | 344   | Confirmada |

  Esquema do Cenário: Validar inscrição alterada: <caso>
    Quando acesso o menu Inscrições
    E seleciono a formação em Inscrições
    E clico no botão de cancelar inscrição
    Então realiza o cancelamento da inscrição manual

    Exemplos:
      | tipo   | valor | caso      |
      | código | 344   | Cancelada |
  
  Esquema do Cenário: Validar inscrição alterada: <caso>
    Quando acesso o menu Inscrições
    E seleciono a formação em Inscrições
    E clico no botão de reativar inscrição
    Então reativa como confirmada a inscrição manual

    Exemplos:
      | tipo   | valor | caso      |
      | código | 344   | Reativada |
  
  Esquema do Cenário: Validar campo obrigatório: <caso>
    Quando acesso o menu Inscrições
    E seleciono a formação em Inscrições
    E tento enviar uma nova inscrição
    Então valida o campo obrigatório na inscrição manual

    Exemplos:
      | campo | caso               |
      | turma | Turma              |
      | rf    | Registro funcional |
      | cargo | Cargo/Função       |