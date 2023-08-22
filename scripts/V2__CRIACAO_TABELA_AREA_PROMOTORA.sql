CREATE TABLE IF NOT EXISTS public.area_promotora (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( NO MINVALUE NO MAXVALUE NO CYCLE),
	nome varchar(50) NOT NULL,
	tipo smallint NOT NULL,
	email varchar(100) NULL,
	grupo_id uuid NOT NULL,
	excluido boolean NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	CONSTRAINT area_promotora_pk PRIMARY KEY (id)
);

CREATE index if not exists area_promotora_excluido_idx ON public.area_promotora (excluido);

CREATE TABLE IF NOT EXISTS public.area_promotora_telefone (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( NO MINVALUE NO MAXVALUE NO CYCLE),
	area_promotora_id int NOT null,
	telefone varchar(12) NOT null,
	excluido boolean NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	CONSTRAINT area_promotora_telefone_pk PRIMARY KEY (id),
	constraint area_promotora_telefone_are_promotora_fk foreign key (area_promotora_id) references public.area_promotora(id)
);

CREATE index if not exists area_promotora_telefone_excluido_idx ON public.area_promotora_telefone (excluido);
