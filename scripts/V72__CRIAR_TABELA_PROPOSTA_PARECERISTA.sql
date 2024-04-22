
CREATE TABLE public.proposta_parecerista (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 NOT NULL,
	registro_funcional varchar(7) not null,
	nome_parecerista varchar(100),
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	CONSTRAINT proposta_parecerista_pk PRIMARY KEY (id)
);
CREATE INDEX if not exists proposta_parecerista_excluido_idx ON public.proposta_parecerista USING btree (excluido);
CREATE INDEX if not exists proposta_id_idx ON public.proposta_parecerista (proposta_id);
CREATE INDEX if not exists registro_funcional_idx ON public.proposta_parecerista (registro_funcional);
CREATE INDEX if not exists nome_pareceristaidx ON public.proposta_parecerista (nome_parecerista);


ALTER TABLE public.proposta_parecerista ADD CONSTRAINT proposta_parecerista_proposta_id_fk FOREIGN KEY (proposta_id) REFERENCES public.proposta(id);

