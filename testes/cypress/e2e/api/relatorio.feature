# language: pt

Funcionalidade: API - Relatório

  Cenário: Gerar relatório de inscritos por formação
    Dado que possuo um token válido no endpoint Relatorio
    Quando envio uma requisição POST em inscritos por formação
    Então retorna o status 202 gerando relatório de inscritos por formação

  Cenário: Campos obrigatórios no relatório de inscritos por formação
    Dado que possuo um token válido no endpoint Relatorio
    Quando envio uma requisição POST sem período no relatório por formação
    Então retorna o status 422 gerando relatório de inscritos por formação

  Cenário: Não gerar relatório de inscritos por formação sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição POST em inscritos por formação
    Então retorna o status 401 sem relatório de inscritos por formação