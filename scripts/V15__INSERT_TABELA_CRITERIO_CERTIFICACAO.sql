insert into public.criterio_certificacao (descricao,criado_em, criado_por, criado_login) 
select 'Conceito P ou S pela participação e envolvimento',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.criterio_certificacao where descricao = 'Conceito P ou S pela participação e envolvimento');

insert into public.criterio_certificacao (descricao,criado_em, criado_por, criado_login) 
select '100% de Frequência',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.criterio_certificacao where descricao = '100% de Frequência');

insert into public.criterio_certificacao (descricao,criado_em, criado_por, criado_login) 
select 'Frequência mínima de 75%',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.criterio_certificacao where descricao = 'Frequência mínima de 75%');

insert into public.criterio_certificacao (descricao,criado_em, criado_por, criado_login) 
select 'Realização de atividade obrigatória',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.criterio_certificacao where descricao = 'Realização de atividade obrigatória');

insert into public.criterio_certificacao (descricao,criado_em, criado_por, criado_login) 
select 'Participação nas aulas síncronas',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.criterio_certificacao where descricao = 'Participação nas aulas síncronas');