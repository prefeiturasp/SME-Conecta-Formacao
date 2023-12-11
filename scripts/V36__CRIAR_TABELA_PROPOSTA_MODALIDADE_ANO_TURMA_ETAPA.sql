create table if not exists public.proposta_modalidade (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 NOT NULL,
	modalidade int2 NOT NULL,		
	excluido bool NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,	
	CONSTRAINT proposta_modalidade_pk PRIMARY KEY (id),
    CONSTRAINT proposta_modalidade_proposta_id_fk FOREIGN KEY (proposta_id) REFERENCES public.proposta(id)	
);

CREATE INDEX if not exists proposta_modalidade_modalidade_idx ON public.proposta_modalidade (modalidade);

create table if not exists public.proposta_ano_turma (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 NOT NULL,
	ano_turma_id int8 NOT NULL,	
	excluido bool NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,	
	CONSTRAINT proposta_ano_turma_etapa_pk PRIMARY KEY (id),
	CONSTRAINT proposta_ano_turma_ano_turma_id_fk FOREIGN KEY (ano_turma_id) REFERENCES public.ano_turma (id),	
	CONSTRAINT proposta_ano_turma_proposta_id_fk FOREIGN KEY (proposta_id) REFERENCES public.proposta(id)
);

create table if not exists public.proposta_componente_curricular (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 NOT NULL,
	componente_curricular_id int8 NOT NULL,	
	excluido bool NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,	
	CONSTRAINT proposta_componente_curricular_pk PRIMARY KEY (id),
	CONSTRAINT proposta_componente_curricular_componente_curricular_id_fk FOREIGN KEY (componente_curricular_id) REFERENCES public.componente_curricular (id),
	CONSTRAINT proposta_componente_curricular_proposta_id_fk FOREIGN KEY (proposta_id) REFERENCES public.proposta(id)
);