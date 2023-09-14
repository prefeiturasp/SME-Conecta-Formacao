CREATE TABLE if not exists public.arquivo (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( NO MINVALUE NO MAXVALUE NO CYCLE),
	nome varchar NOT NULL,
	codigo uuid NOT NULL,
	tipo int4 NOT NULL,
	tipo_conteudo varchar NOT NULL,
	excluido bool NOT NULL DEFAULT false,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	CONSTRAINT arquivo_pk PRIMARY KEY (id)
);

alter table proposta add if not exists arquivo_imagem_divulgacao_id int8;