alter table proposta add if not exists data_realizacao_inicio timestamp;
alter table proposta add if not exists data_realizacao_fim timestamp;

alter table proposta add if not exists data_inscricao_inicio timestamp;
alter table proposta add if not exists data_inscricao_fim timestamp;

create table if not exists public.proposta_encontro (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 not null,
	hora_inicio varchar(5) null,
	hora_fim varchar(5) null,
	tipo smallint null,
	local varchar(200) null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT null,
	constraint proposta_encontro_pk primary key (id),
	constraint proposta_encontro_proposta_fk foreign key (proposta_id) references proposta (id)
);

create table if not exists public.proposta_encontro_turma (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_encontro_id int8 not null,
	turma smallint null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT null,
	constraint proposta_encontro_turma_pk primary key (id),
	constraint proposta_encontro_turma_proposta_encontro_fk foreign key (proposta_encontro_id) references proposta_encontro (id)
);

create table if not exists public.proposta_encontro_data (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_encontro_id int8 not null,
	data_inicio timestamp null,
	data_fim timestamp null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT null,
	constraint proposta_encontro_data_pk primary key (id),
	constraint proposta_encontro_data_proposta_encontro_fk foreign key (proposta_encontro_id) references proposta_encontro (id)	
);

CREATE index if not exists proposta_encontro_excluido_idx ON public.proposta_encontro (excluido);
CREATE index if not exists proposta_encontro_turma_excluido_idx ON public.proposta_encontro_turma (excluido);
CREATE index if not exists proposta_encontro_data_excluido_idx ON public.proposta_encontro_data (excluido);



