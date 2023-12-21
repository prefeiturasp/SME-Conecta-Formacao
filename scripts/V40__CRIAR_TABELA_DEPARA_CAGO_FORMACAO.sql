CREATE TABLE if not exists public.cargo_funcao_depara_eol (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	cargo_funcao_id int8 not null,
	codigo_cargo_eol int8 null,
	codigo_funcao_eol int8 null,
	excluido bool NOT NULL DEFAULT false,
	CONSTRAINT cargo_funcao_depara_eol_pk PRIMARY KEY (id),
	CONSTRAINT cargo_funcao_depara_eol_cargo_funcao_fk FOREIGN KEY (cargo_funcao_id) REFERENCES cargo_funcao (id)
);