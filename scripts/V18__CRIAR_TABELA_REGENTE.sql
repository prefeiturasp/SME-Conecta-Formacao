CREATE table if not exists public.proposta_regente (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 NOT NULL,
	profissional_rede_municipal bool NOT NULL,
	registro_funcional varchar(7) NULL,
	nome_regente varchar(50) NULL,
	mini_biografia text NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	CONSTRAINT proposta_regente_pk PRIMARY KEY (id)
);
CREATE INDEX proposta_regente_excluido_idx ON public.proposta_regente USING btree (excluido);

ALTER TABLE public.proposta_regente ADD CONSTRAINT proposta_regente_proposta_fk FOREIGN KEY (proposta_id) REFERENCES public.proposta(id);



CREATE TABLE if not exists public.proposta_regente_turma (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_regente_id int8 NOT NULL,
	turma int2 NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	CONSTRAINT proposta_regente_turma_pk PRIMARY KEY (id)
);
CREATE INDEX proposta_regente_turma_excluido_idx ON public.proposta_regente_turma USING btree (excluido);
ALTER TABLE public.proposta_regente_turma ADD CONSTRAINT proposta_proposta_regente_turma_regente_fk FOREIGN KEY (proposta_regente_id) REFERENCES public.proposta_regente(id);