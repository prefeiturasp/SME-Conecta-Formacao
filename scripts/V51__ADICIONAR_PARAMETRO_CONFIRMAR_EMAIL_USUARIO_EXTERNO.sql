insert into parametro_sistema (nome, tipo, descricao, valor, ano, ativo, criado_em, criado_por, criado_login)
select 'ConfirmarEmailUsuarioExterno', 5, 'Manda e-mail de confirmação para ativar cadastro','false', 2024, true, now(), 'Sistema', 'Sistema' 
where not exists (select id from parametro_sistema where ano = 2024 and tipo = 5);