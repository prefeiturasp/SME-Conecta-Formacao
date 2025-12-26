CREATE TABLE IF NOT EXISTS codaf_lista_presenca(
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 NOT NULL,
	proposta_turma_id int8 NOT NULL,
	data_publicacao date NULL,
	data_publicacao_dom date NULL,
	numero_comunicado SMALLINT NULL,
	pagina_comunicado_dom SMALLINT NULL,
	codigo_curso_eol int NULL,
	codigo_nivel int NULL,
	observacao text NULL,
	data_envio_df timestamptz NULL,
	status int NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	
	CONSTRAINT codaf_lista_presenca_pk PRIMARY KEY (id),
	CONSTRAINT codaf_lista_presenca_prop_id_fk FOREIGN KEY (proposta_id) REFERENCES PROPOSTA,
	CONSTRAINT codaf_lista_presenca_prop_turma_id_fk FOREIGN KEY (proposta_turma_id) REFERENCES PROPOSTA_TURMA,
	CONSTRAINT proposta_turma_id_key UNIQUE (proposta_turma_id)
);

CREATE TABLE IF NOT EXISTS codaf_inscricao_lista_presenca(
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	codaf_lista_presenca_id int8 NOT NULL,
	inscricao_id int8 NOT NULL,
	percentual_frequencia numeric(5, 2) NULL,
	atividade_obrigatorio boolean NULL,
	conceito_final varchar(2) NULL,
	aprovado boolean NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	
	CONSTRAINT codaf_inscricao_lista_presenca_pk PRIMARY KEY (id),
	CONSTRAINT codaf_inscricao_lista_presenca_codaf_lista_presenca_id_fk FOREIGN KEY (codaf_lista_presenca_id) REFERENCES CODAF_LISTA_PRESENCA,
	CONSTRAINT codaf_inscricao_lista_presenca_inscricao_id_fk FOREIGN KEY (inscricao_id) REFERENCES INSCRICAO,
	CONSTRAINT codaf_inscricao_lista_presenca_inscricao_id_codaf_lista_presenca_id_key UNIQUE(codaf_lista_presenca_id, inscricao_id)
);

CREATE TABLE IF NOT EXISTS codaf_retificacao_lista_presenca(
	id int NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	codaf_lista_presenca_id int8 NOT NULL,
	data_retificacao date NOT NULL,
	pagina_retificacao_dom SMALLINT NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	
	CONSTRAINT codaf_retificacao_lista_presenca_pk PRIMARY KEY (id),
	CONSTRAINT codaf_retificacao_lista_presenca_codaf_lista_presenca_id_fk FOREIGN KEY (codaf_lista_presenca_id) REFERENCES CODAF_LISTA_PRESENCA
);

CREATE TABLE IF NOT EXISTS codaf_comentario_lista_presenca(
	id int NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	codaf_lista_presenca_id int8 NOT NULL,
	comentario text NOT NULL,
	notificacao_enviada bool NOT NULL,
	data_notificacao timestamptz,
	ativo bool NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	
	CONSTRAINT codaf_comentario_lista_presenca_pk PRIMARY KEY (id),
	CONSTRAINT codaf_comentario_lista_presenca_codaf_lista_presenca_id_fk FOREIGN KEY (codaf_lista_presenca_id) REFERENCES CODAF_LISTA_PRESENCA
);


CREATE INDEX IF NOT EXISTS idx_proposta_numero_homologacao_autocomplete 
ON proposta (CAST(NUMERO_HOMOLOGACAO AS TEXT) text_pattern_ops);

CREATE INDEX IF NOT EXISTS idx_proposta_numero_homologacao_puro ON proposta (NUMERO_HOMOLOGACAO);