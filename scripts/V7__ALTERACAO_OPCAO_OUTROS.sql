alter table criterio_validacao_inscricao add outros boolean default ('false');
alter table criterio_validacao_inscricao add ordem smallint;
update criterio_validacao_inscricao set ordem = id where ordem is null;
insert into criterio_validacao_inscricao (nome, unico, outros, ordem, criado_em, criado_por, criado_login, excluido) values ('Outros', false, true, 999, now(), 'Sistema', 'Sistema', false);

alter table cargo_funcao add outros boolean default ('false');
alter table cargo_funcao add ordem smallint;
update cargo_funcao set ordem = id where ordem is null;
insert into cargo_funcao (nome, tipo, outros, ordem, criado_em, criado_por, criado_login, excluido) values ('Outros', 3, true, 999, now(), 'Sistema', 'Sistema', false);
