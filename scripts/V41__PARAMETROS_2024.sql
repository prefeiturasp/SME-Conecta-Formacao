--> Inserindo parâmetros de 2023 em 2024
insert into parametro_sistema (nome, tipo, descricao,valor,ano,ativo,criado_em, criado_por, criado_login)
select nome, tipo, descricao,valor,2024,ativo,criado_em, criado_por, criado_login 
from parametro_sistema 
where ano = 2023
and not exists (select id from parametro_sistema where ano = 2024);