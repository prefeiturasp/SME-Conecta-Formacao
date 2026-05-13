# language: pt

Funcionalidade: Consulta de Área Promotora

  Contexto:
    Dado eu acesso o sistema com a visualização web
    E realizo login no sistema Conecta Formação com perfil "Admin"

  Cenário: Validar exibição dos resultados
    Quando visualizo a tela "Área Promotora"
    Então sistema apresenta o resultado da consulta de área promotora

  Esquema do Cenário: Validar consulta por tipo ao: <caso>
    Quando visualizo a tela "Área Promotora"
    E seleciono o tipo "<tipo>"
    Então sistema apresenta o resultado da consulta de área promotora com tipo "<tipo>"

    Exemplos:
      | tipo          | caso                   |
      | Rede Parceria | Parceria |
      | Rede Direta   | Direta   |

  Esquema do Cenário: Validar consulta por nome ao: <caso>
    Quando visualizo a tela "Área Promotora"
    E preencho o nome da promotora com "<nome>"
    Então sistema apresenta o resultado da consulta de área promotora com "<nome>"

    Exemplos:
      | nome  | caso               |
      | teste | Consultar por nome |