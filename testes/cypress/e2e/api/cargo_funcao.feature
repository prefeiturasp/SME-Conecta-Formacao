# language: pt

Funcionalidade: API - Cargo função

  Cenário: Buscar cargo função
    Dado que possuo um token no endpoint CargoFuncao
    Quando envio uma requisição GET em cargos
    Então retorna todos cargo função com status 200

  Cenário: Buscar cargo função exibindo a opção de outros
    Dado que possuo um token no endpoint CargoFuncao
    Quando envio uma requisição GET em cargos com true
    Então retorna todos cargo função exibindo a opção de outros com status 200

  Cenário: Buscar cargo função não exibindo a opção de outros
    Dado que possuo um token no endpoint CargoFuncao
    Quando envio uma requisição GET em cargos com false
    Então retorna todos cargo função não exibindo a opção de outros com status 200

  Cenário: Não buscar cargo função sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET no endpoint de cargos
    Então retorna o status 401 sem todos cargo função

  Cenário: Buscar cargo função do tipo 1
    Dado que possuo um token no endpoint CargoFuncao
    Quando envio uma requisição GET em cargos de tipo
    Então retorna cargo função do tipo 1 com status 200

  Cenário: Buscar cargo função do tipo 2
    Dado que possuo um token no endpoint CargoFuncao
    Quando envio uma requisição GET em cargos do tipo
    Então retorna cargo função do tipo 2 com status 200

  Cenário: Buscar cargo função do tipo 3
    Dado que possuo um token no endpoint CargoFuncao
    Quando envio uma requisição GET em cargos tipo
    Então retorna cargo função do tipo 3 com status 200

  Cenário: Tipo é obrigatório em cargo função
    Dado que possuo um token no endpoint CargoFuncao
    Quando envio uma requisição GET em cargos sem tipo
    Então retorna que tipo é obrigatório em cargo função com status 404

  Cenário: Buscar cargo função tipo exibindo a opção de outros
    Dado que possuo um token no endpoint CargoFuncao
    Quando envio uma requisição GET em cargos tipo com true
    Então retorna todos cargo função tipo exibindo a opção de outros com status 200

  Cenário: Buscar cargo função tipo não exibindo a opção de outros
    Dado que possuo um token no endpoint CargoFuncao
    Quando envio uma requisição GET em cargos tipo com false
    Então retorna todos cargo função tipo não exibindo a opção de outros com status 200

  Cenário: Não buscar cargo função tipo sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET no endpoint de cargos tipos
    Então retorna o status 401 sem todos cargo função tipo