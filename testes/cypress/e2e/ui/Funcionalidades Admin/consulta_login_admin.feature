# language: pt

Funcionalidade: Acesso de usuário admin

  Contexto:
    Validação dos cenários e campos obrigatórios
  
  Esquema do Cenário: Validar: <caso>
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"
    Então o sistema valida o "<campo>" no acesso

    Exemplos:
      | campo | caso              |
      | login | Login com sucesso |

  Esquema do Cenário: Validar: <caso>
    Dado eu acesso o sistema Conecta Formação
    Quando clico em entrar na tela de login
    Então o sistema valida "<campo>" como obrigatório no acesso

    Exemplos:
      | campo       | caso                       |
      | login       | Campo de login obrigatório |
      | senha_admin | Campo de senha obrigatório |

  Esquema do Cenário: Validar: <caso>
    Dado eu acesso o sistema Conecta Formação
    Quando clico em entrar na tela de login
    Então o sistema valida a quantidade "<campo>" de caracteres com o valor "<dado>" no acesso

    Exemplos:
      | dado | campo | caso                            |
      | 1234 | login | mínimo de 5 caracteres no login |
      | 123  | senha | mínimo de 4 caracteres na senha |

  Esquema do Cenário: Validar: <caso>
    Dado eu acesso o sistema Conecta Formação
    Quando clico em entrar na tela de login
    Então o sistema valida "<campo>" inválido "<dado>" no acesso

    Exemplos:
      | dado  | campo   | caso             |
      | 12345 | login   | Usuário inválido |
      | 1234  | senha   | Senha inválida   |