--> Criação da tabela de proposta_parecer
CREATE TABLE if not exists public.proposta_parecer (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 NOT NULL,
	campo int2 NOT NULL,	
	descricao varchar(1000) NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	CONSTRAINT proposta_parecer_pk PRIMARY KEY (id),
	CONSTRAINT proposta_parecer_proposta_id_fk FOREIGN KEY (proposta_id) REFERENCES public.proposta(id)
);

--> Criando índice de proposta em proposta_parecer
CREATE INDEX if not exists proposta_parecer_proposta_idx ON public.proposta_parecer (proposta_id);