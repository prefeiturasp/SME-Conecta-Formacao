# language: pt

Funcionalidade: API - Área Promotora

  Cenário: Buscar tipos da Área Promotora
    Dado que possuo um token válido no endpoint AreaPromotora
    Quando envio uma requisição GET de tipos da promotora
    Então retorna o status 200 com tipos da Área Promotora

  Cenário: Não buscar tipos da Área Promotora sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET de tipos da promotora
    Então retorna o status 401 sem tipos da Área Promotora

  Cenário: Buscar Área Promotora
    Dado que possuo um token válido no endpoint AreaPromotora
    Quando envio uma requisição GET de promotora
    Então retorna o status 200 com Área Promotora

  Cenário: Não buscar Área Promotora sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET de Área Promotora
    Então retorna o status 401 sem Área Promotora

  Cenário: Buscar lista Área Promotora
    Dado que possuo um token válido no endpoint AreaPromotora
    Quando envio uma requisição GET de lista promotora
    Então retorna o status 200 com lista Área Promotora

  Cenário: Não buscar lista Área Promotora sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET de lista Área Promotora
    Então retorna o status 401 sem lista Área Promotora

  Cenário: Buscar lista rede parceira Área Promotora
    Dado que possuo um token válido no endpoint AreaPromotora
    Quando envio uma requisição GET de lista parceira promotora
    Então retorna o status 200 com lista rede parceira Área Promotora

  Cenário: Não buscar lista rede parceira Área Promotora sem autenticação
    Dado que não possuo um token válido
    Quando tento a requisição GET de lista parceira promotora
    Então retorna o status 401 sem lista rede parceira Área Promotora

  #Cenário: Cadastrar Área Promotora com sucesso
  #  Dado que possuo um token válido no endpoint AreaPromotora
  #  Quando envio uma requisição POST para cadastrar Área Promotora
  #  Então retorna sucesso no cadastro da Área Promotora

  Cenário: Não cadastrar Área Promotora sem payload
    Dado que possuo um token válido no endpoint AreaPromotora
    Quando envio uma requisição POST sem payload para Área Promotora
    Então retorna erro 422 ao cadastrar Área Promotora

  #Cenário: Não cadastrar Área Promotora com label já existente
  #  Dado que possuo um token válido no endpoint AreaPromotora
  #  Quando crio um registro e tento cadastrar novamente a mesma label de Área Promotora
  #  Então não cadastra Área Promotora com label duplicada retornando o status 400

  Cenário: Não cadastrar Área Promotora sem autenticação
    Dado que não possuo um token válido
    Quando tento enviar uma requisição POST para cadastrar Área Promotora
    Então retorna o status 401 ao cadastrar Área Promotora

  #Cenário: Excluir Área Promotora
  #  Dado que possuo um token válido no endpoint AreaPromotora
  #  Quando crio um registro para validar o delete de Área Promotora
  #  Então exclui a Área Promotora com o status 200

   Cenário: Não exclui Área Promotora sem id
    Dado que não possuo um token válido
    Quando tento deletar Área Promotora sem id
    Então não exlcui Área Promotora sem id retornando o status 400

  Cenário: Não deletar Área Promotora
    Dado que não possuo um token válido
    Quando tento deletar Área Promotora
    Então não exlcui Área Promotora retornando o status 400

  #Cenário: Editar Área Promotora por id
  #  Dado que possuo um token válido no endpoint AreaPromotora
  #  Quando crio um registro para validar a edição por id de Área Promotora
  #  Então edito a Área Promotora por id com o status 200

  Cenário: Não editar Área Promotora sem id
    Dado que possuo um token válido no endpoint AreaPromotora
    Quando tento editar Área Promotora sem id
    Então não edita Área Promotora sem id retornando o status 400

  Cenário: Não editar Área Promotora com token inválido
    Dado que não possuo um token válido
    Quando tento editar Área Promotora com token inválido
    Então não edita Área Promotora retornando o status 401

  #Cenário: Buscar Área Promotora por id
  #  Dado que possuo um token válido no endpoint AreaPromotora
  #  Quando crio um registro para validar a busca por id de Área Promotora
  #  Então busco a Área Promotora por id com o status 200

  Cenário: Não buscar Área Promotora sem id válido
    Dado que possuo um token válido no endpoint AreaPromotora
    Quando tento buscar Área Promotora com id inválido
    Então não busca Área Promotora retornando o status 400

  Cenário: Não buscar Área Promotora com token inválido
    Dado que não possuo um token válido
    Quando tento buscar Área Promotora com token inválido
    Então não busca Área Promotora retornando o status 401