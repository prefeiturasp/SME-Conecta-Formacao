--> Criação da tabela de anos das turmas
CREATE TABLE if not exists public.ano (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	codigo_eol varchar(1) not NULL,	
	descricao varchar(70) not null,
	codigo_serie_ensino int8 not null,
	modalidade int8 not null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	CONSTRAINT ano_pk PRIMARY KEY (id)	
);
CREATE INDEX if not exists ano_codigo_serie_ensino_idx ON public.ano (codigo_serie_ensino);
CREATE INDEX if not exists ano_modalidade_idx ON public.ano (modalidade);
CREATE INDEX if not exists ano_codigo_eol_idx ON public.ano (codigo_eol);

--> Criação da tabela de Componentes curriculares
CREATE TABLE if not exists public.componente_curricular (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	ano_id int8 not null,	
	codigo_eol int8 not NULL,	
	nome varchar(70) not null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	CONSTRAINT componente_curricular_pk PRIMARY KEY (id),
	CONSTRAINT componente_curricular_ano_id_fk FOREIGN KEY (ano_id) REFERENCES public.ano(id)
);
CREATE INDEX if not exists componente_curricular_codigo_eol_idx ON public.componente_curricular(codigo_eol);