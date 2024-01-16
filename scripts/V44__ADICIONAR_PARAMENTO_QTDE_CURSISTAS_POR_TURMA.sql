--> Inserindo parâmetros 'QtdeCursistasSuportadosPorTurma'
insert into parametro_sistema (nome, tipo, descricao,valor,ano,ativo,criado_em, criado_por, criado_login)
select 'QtdeCursistasSuportadosPorTurma', 3, 'Quantidade de cursistas suportados por turma na inscrição automática','950',2024,true,now(), 'Sistema', 'Sistema' 
where not exists (select id from parametro_sistema where ano = 2024 and tipo = 3);