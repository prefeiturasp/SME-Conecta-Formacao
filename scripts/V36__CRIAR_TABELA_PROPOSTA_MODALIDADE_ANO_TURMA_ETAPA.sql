create table if not exists public.proposta_modalidade_ano_turma_etapa (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	modalidade int2 NOT NULL,
	ano_turma_id int8 NOT NULL,
	etapa_eja int2 NULL,
	excluido bool NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,	
	CONSTRAINT proposta_modalidade_ano_turma_etapa_pk PRIMARY KEY (id),
	CONSTRAINT proposta_modalidade_ano_turma_etapa_ano_turma_id_fk FOREIGN KEY (ano_turma_id) REFERENCES public.ano_turma (id)
);

CREATE INDEX if not exists proposta_modalidade_ano_turma_etapa_modalidade_idx ON public.proposta_modalidade_ano_turma_etapa (ano_turma_id);