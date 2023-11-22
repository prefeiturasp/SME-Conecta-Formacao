CREATE TABLE if not exists public.proposta_justificativa_parecer (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 not NULL,	
	parecer varchar(1000) NULL,
	situacao smallint null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	CONSTRAINT proposta_justificativa_parecer_pk PRIMARY KEY (id),
	constraint proposta_justificativa_parecer_fk foreign key (proposta_id) references proposta (id)
);
CREATE INDEX if not exists proposta_justificativa_parecer_id_idx ON public.proposta_justificativa_parecer (id);
CREATE INDEX if not exists proposta_justificativa_parecer_proposta_id_idx ON public.proposta_justificativa_parecer (proposta_id);