CREATE table if not exists public.proposta_tutor (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 NOT NULL,
	profissional_rede_municipal bool NOT NULL,
	registro_funcional varchar(7) NULL,
	nome_tutor varchar(50) NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	CONSTRAINT proposta_tutor_pk PRIMARY KEY (id)
);
CREATE INDEX proposta_tutor_excluido_idx ON public.proposta_tutor USING btree (excluido);

ALTER TABLE public.proposta_tutor ADD CONSTRAINT proposta_tutor_proposta_fk FOREIGN KEY (proposta_id) REFERENCES public.proposta(id);



CREATE TABLE if not exists public.proposta_tutor_turma (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_tutor_id int8 NOT NULL,
	turma int2 NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	CONSTRAINT proposta_tutor_turma_pk PRIMARY KEY (id)
);
CREATE INDEX proposta_tutor_turma_excluido_idx ON public.proposta_tutor_turma USING btree (excluido);
ALTER TABLE public.proposta_tutor_turma ADD CONSTRAINT proposta_proposta_tutor_turma_tutor_fk FOREIGN KEY (proposta_tutor_id) REFERENCES public.proposta_tutor(id);