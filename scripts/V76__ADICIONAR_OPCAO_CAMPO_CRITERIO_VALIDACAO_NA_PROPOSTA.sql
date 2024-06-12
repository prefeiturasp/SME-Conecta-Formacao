insert into criterio_validacao_inscricao (nome, unico, criado_em, criado_por, criado_login, excluido) 
select 'Caso o número de inscritos ultrapasse o número de vagas será realizado sorteio', false, now(), 'Sistema', 'Sistema', false
where not exists (select id from criterio_validacao_inscricao where nome = 'Caso o número de inscritos ultrapasse o número de vagas será realizado sorteio');