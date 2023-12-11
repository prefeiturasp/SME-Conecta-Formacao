CREATE TABLE public.dre (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	dre_id varchar(15) NULL,
	abreviacao varchar(10) NULL,
	nome varchar(100) NULL,
	data_atualizacao timestamp NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	CONSTRAINT dre_pk PRIMARY KEY (id)
);
CREATE INDEX dre_dre_id_idx ON public.dre (dre_id);