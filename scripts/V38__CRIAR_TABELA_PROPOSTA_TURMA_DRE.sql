CREATE TABLE if not exists public.proposta_turma_dre (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_turma_id int8 not NULL,		
	dre_id int8 NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL DEFAULT false,
	CONSTRAINT proposta_turma_dre_pk PRIMARY KEY (id),
	constraint proposta_turma_dre_proposta_turma_fk foreign key (proposta_turma_id) references proposta_turma (id),
	CONSTRAINT proposta_turma_dre_id_fk FOREIGN KEY (dre_id) REFERENCES public.dre(id)
);
CREATE INDEX if not exists proposta_turma_dre_proposta_turma_id_idx ON public.proposta_turma_dre (id);

alter table proposta_turma drop column  if exists dre_id;