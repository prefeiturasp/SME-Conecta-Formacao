create table if not exists public.criterio_certificacao (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	descricao varchar(100) not null,	
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT null default false,
	constraint criterio_certificacao_pk primary key (id)	
);

create table if not exists public.proposta_criterio_certificacao (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 not null,
	criterio_certificacao_id int8 not null,	
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT NULL,
	constraint proposta_criterio_certificacao_pk primary key (id),
	constraint proposta_criterio_certificacao_proposta_fk foreign key (proposta_id) references proposta (id),
	constraint proposta_criterio_certificacao_criterio_certificacao_fk foreign key (criterio_certificacao_id) references criterio_certificacao (id)	
);

CREATE index if not exists proposta_criterio_certificacao_proposta_idx ON public.proposta (id);
CREATE index if not exists proposta_criterio_certificacao_criterio_certificacao_idx ON public.criterio_certificacao (id);

insert into public.criterio_certificacao (descricao,criado_em, criado_por, criado_login) 
select 'Conceito P ou S pela participação e envolvimento',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.criterio_certificacao where descricao = 'Conceito P ou S pela participação e envolvimento') union all
select '100% de Frequência',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.criterio_certificacao where descricao = '100% de Frequência') union all
select 'Frequência mínima de 75%',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.criterio_certificacao where descricao = 'Frequência mínima de 75%') union all
select 'Realização de atividade obrigatória',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.criterio_certificacao where descricao = 'Realização de atividade obrigatória') union all
select 'Participação nas aulas síncronas',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.criterio_certificacao where descricao = 'Participação nas aulas síncronas') union all
