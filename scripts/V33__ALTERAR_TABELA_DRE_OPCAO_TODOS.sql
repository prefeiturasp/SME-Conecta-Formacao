alter table dre add todos bool default('false');
alter table dre add ordem int2;
update dre set ordem = 1;
insert into dre (abreviacao, nome, data_atualizacao, criado_em, criado_por, criado_login, excluido, ordem, todos) values ('TODOS', 'TODOS', now(), now(), 'Sistema', 'Sistema', false, 0, true);