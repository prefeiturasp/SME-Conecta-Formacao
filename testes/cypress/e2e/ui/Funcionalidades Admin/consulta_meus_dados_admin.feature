# language: pt

Funcionalidade: Consulta de Meus Dados

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"

    Esquema do Cenário: Validar campo preenchido: <caso>
    Quando acesso o menu Meus Dados
    Então os campos de Meus Dados devem estar preenchidos para "<tipo>"

    Exemplos:
      | tipo               | caso                        |
      | nome               | Nome usuário                |
      | email              | E-mail                      |
      | tipo               | Tipo                        |
      | email educacional  | E-mail Educacional          |
      | pessoa deficiencia | Pessoa com deficiência      |
      | senha              | Senha                       |
