# language: pt

Funcionalidade: Consulta do Relatório de inscritos por formação

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"

  Esquema do Cenário: Gerar o relatório com sucesso
    Quando acesso o menu Relatório de inscritos por formação
    E preencho para gerar o relatório de inscritos
    Então gera o relatório com sucesso de inscritos por formação

  Esquema do Cenário: Campos obrigatórios ao gerar relatório
    Quando acesso o menu Relatório de inscritos por formação
    E não preencho para gerar o relatório de inscritos
    Então não gera o relatório de inscritos por formação

  Esquema do Cenário: Validar filtro na aba formação: <caso>
    Quando acesso o menu Relatório de inscritos por formação
    Então retorna os campos de inscritos por formação para "<tipo>"

    Exemplos:
      | tipo        | caso                    |
      | formação    | Código formação         |
      | homologação | Código homologação      |
      | turma       | Turma                   |
      | modalidade  | Modalidade formativa    |
      | nome        | Nome da formação        |
      | área        | Área promotora          |
      | situação    | Situação das inscrições |
    
