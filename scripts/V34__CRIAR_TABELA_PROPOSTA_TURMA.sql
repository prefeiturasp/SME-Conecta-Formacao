create table if not exists public.proposta_turma (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 NOT NULL,
	nome varchar(50) NOT NULL,
	dre_id int8 NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	CONSTRAINT proposta_turma_pk PRIMARY KEY (id),
	CONSTRAINT proposta_turma_proposta_id_fk FOREIGN KEY (proposta_id) REFERENCES public.proposta (id),
	CONSTRAINT proposta_turma_dre_id_fk FOREIGN KEY (dre_id) REFERENCES public.dre (id)
);

CREATE INDEX if not exists proposta_turma_proposta_id_idx ON public.proposta_turma (proposta_id);