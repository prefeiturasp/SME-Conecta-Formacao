--> Tabela de importação de arquivos
CREATE TABLE if not exists public.importacao_arquivo (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE),
	proposta_id int8 not null,
	nome varchar(200) NOT NULL, 
	tipo int2 NOT NULL,
	situacao int2 NOT NULL,
	excluido bool NOT NULL DEFAULT false,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	CONSTRAINT importacao_arquivo_pk PRIMARY KEY (id),
	CONSTRAINT importacao_arquivo_proposta_fk FOREIGN KEY (proposta_id) REFERENCES public.proposta (id)
);

--> Tabela de itens da importação 
CREATE TABLE if not exists public.importacao_arquivo_registro (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE),
	importacao_arquivo_id int8 not null,	
	linha int2 NOT NULL,		
	conteudo jsonb NOT NULL, 
	situacao int2 NOT NULL,
	erro text null,
	excluido bool NOT NULL DEFAULT false,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	CONSTRAINT importacao_arquivo_registro_pk PRIMARY KEY (id),
	CONSTRAINT importacao_arquivo_registro_importacao_arquivo_fk FOREIGN KEY (importacao_arquivo_id) REFERENCES public.importacao_arquivo (id)
);