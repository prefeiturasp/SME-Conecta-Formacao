CREATE TABLE IF NOT EXISTS public.proposta (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( NO MINVALUE NO MAXVALUE NO CYCLE),
	tipo_formacao smallint null,
	modalidade smallint null,
	tipo_inscricao smallint null,
	nome_formacao varchar(150) null,
	quantidade_turmas smallint null,
	quantidade_vagas_turma smallint null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT NULL,
	CONSTRAINT proposta_pk PRIMARY KEY (id)
);

create table if not exists public.proposta_publico_alvo (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 not null,
	cargo_funcao_id int8 not null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT NULL,
	CONSTRAINT proposta_publico_alvo_pk PRIMARY KEY (id),
	constraint proposta_publico_alvo_proposta_fk foreign key (proposta_id) references public.proposta(id),
	constraint proposta_publico_alvo_cargo_funcao_fk foreign key (cargo_funcao_id) references public.cargo_funcao(id)
);

create table if not exists public.proposta_funcao_especifica (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 not null,
	cargo_funcao_id int8 not null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT NULL,
	CONSTRAINT proposta_funcao_especifica_pk PRIMARY KEY (id),
	constraint proposta_funcao_especifica_proposta_fk foreign key (proposta_id) references public.proposta(id),
	constraint proposta_funcao_especifica_cargo_funcao_fk foreign key (cargo_funcao_id) references public.cargo_funcao(id)
);

create table if not exists public.proposta_criterio_valiacao_inscricao (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 not null,
	criterio_validacao_inscricao_id int8 not null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT NULL,
	CONSTRAINT proposta_criterio_valiacao_inscricao_pk PRIMARY KEY (id),
	constraint proposta_criterio_valiacao_inscricao_proposta_fk foreign key (proposta_id) references public.proposta(id),
	constraint proposta_criterio_valiacao_inscricao_crit_valiacao_prop_fk foreign key (criterio_validacao_inscricao_id) references public.criterio_validacao_inscricao(id)
);

create table if not exists public.proposta_vaga_remanecente (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 not null,
	cargo_funcao_id int8 not null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT NULL,
	CONSTRAINT proposta_vaga_remanecente_pk PRIMARY KEY (id),
	constraint proposta_vaga_remanecente_proposta_fk foreign key (proposta_id) references public.proposta(id),
	constraint proposta_vaga_remanecente_cargo_funcao_fk foreign key (cargo_funcao_id) references public.cargo_funcao(id)
);

CREATE index if not exists proposta_excluido_idx ON public.usuario (excluido);