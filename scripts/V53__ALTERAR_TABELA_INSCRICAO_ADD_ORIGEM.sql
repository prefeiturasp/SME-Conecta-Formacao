alter table inscricao add if not exists origem smallint;  

update inscricao set origem = 1
where origem is null and criado_por = 'Sistema';

update inscricao set origem = 2
where origem is null and criado_por <> 'Sistema';