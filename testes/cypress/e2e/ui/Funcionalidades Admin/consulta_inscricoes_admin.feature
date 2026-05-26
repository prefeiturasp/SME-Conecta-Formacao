# language: pt

Funcionalidade: Consulta de Inscrições

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"

  Esquema do Cenário: Filtrar formações através do: <caso>
    Quando acesso o menu Inscrições
    E preencho o campo "<tipo>" com "<valor>" nas inscrições
    Então exibe os campos de Inscrições "<tipo>"

    Exemplos:
      | tipo        | valor | caso                  |
      | código      | 344   | Código da formação    |
      | nome        | Teste | Nome da formação      |
      | homologação | 0     | Número de homologação |

  Esquema do Cenário: Buscar na listagem de inscrições por: <caso>
    Quando acesso o menu Inscrições
    E preencho o campo "<tipo>" com "<valor>" nas inscrições
    Então busca na listagem em inscrições "<campo>"

    Exemplos:
      | campo     | tipo   | valor | caso               |
      | turma     | código | 344   | Turma              |
      | cargo     | código | 344   | Cargo/função       |
      | situação  | código | 344   | Situação           |
      | rf        | código | 344   | Registro funcional |
      | documento | código | 344   | Documento          |
      | nome      | código | 344   | Nome do cursista   |