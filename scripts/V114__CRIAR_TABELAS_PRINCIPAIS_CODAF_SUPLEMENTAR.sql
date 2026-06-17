CREATE TABLE IF NOT EXISTS codaf_suplementar(
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	codaf_lista_presenca_id int8 NOT NULL,
	data_publicacao timestamp without time zone NULL,
	data_publicacao_dom timestamp without time zone NULL,
	numero_comunicado SMALLINT NULL,
	pagina_comunicado_dom SMALLINT NULL,
	codigo_curso_eol int NULL,
	codigo_nivel int NULL,
	observacao text NULL,
	status int NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	
	CONSTRAINT codaf_suplementar_pk PRIMARY KEY (id),
	CONSTRAINT codaf_suplementar_codaf_id_fk FOREIGN KEY (codaf_lista_presenca_id) REFERENCES CODAF_LISTA_PRESENCA,
	CONSTRAINT codaf_lista_presenca_id_key UNIQUE (codaf_lista_presenca_id)
);

CREATE TABLE IF NOT EXISTS codaf_suplementar_inscricao(
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	codaf_suplementar_id int8 NOT NULL,
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
	
	CONSTRAINT codaf_suplementar_inscricao_pk PRIMARY KEY (id),
	CONSTRAINT codaf_suplementar_inscricao_codaf_suplementar_id_fk FOREIGN KEY (codaf_suplementar_id) REFERENCES CODAF_SUPLEMENTAR,
	CONSTRAINT codaf_suplementar_inscricao_inscricao_id_fk FOREIGN KEY (inscricao_id) REFERENCES INSCRICAO,
	CONSTRAINT codaf_suplementar_inscricao_codaf_suplementar_id_key UNIQUE(codaf_suplementar_id, inscricao_id)
);

CREATE TABLE IF NOT EXISTS codaf_suplementar_retificacao(
	id int NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	codaf_suplementar_id int8 NOT NULL,
	data_retificacao date NOT NULL,
	pagina_retificacao_dom SMALLINT NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	
	CONSTRAINT codaf_suplementar_retificacao_pk PRIMARY KEY (id),
	CONSTRAINT codaf_suplementar_retificacao_codaf_suplementar_id_fk FOREIGN KEY (codaf_suplementar_id) REFERENCES CODAF_SUPLEMENTAR
);

CREATE TABLE IF NOT EXISTS codaf_suplementar_anexo (
    id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
    codaf_suplementar_id int8 NOT NULL,    
    arquivo_codigo UUID NOT NULL, 
    nome_arquivo VARCHAR(255) NOT NULL, 
    extensao VARCHAR(10) NOT NULL,
    tipo_anexo_id INT NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido BOOLEAN DEFAULT FALSE NOT NULL,
	
	CONSTRAINT codaf_suplementar_anexo_pk PRIMARY KEY (id),
    CONSTRAINT codaf_suplementar_anexo_codaf_suplementar_id_fk FOREIGN KEY (codaf_suplementar_id) REFERENCES codaf_suplementar(id)
);

-- Criação da tabela de log
CREATE TABLE IF NOT EXISTS codaf_suplementar_log_remessa_conclusao (
    id SERIAL PRIMARY KEY,
    codaf_suplementar_id BIGINT NOT NULL,
    criado_login varchar(200) NOT NULL,              -- Quem gerou (Auditoria)
    data_geracao TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL,
    hash_arquivo VARCHAR(64) NOT NULL,       -- SHA256
    quantidade_registros INT NOT NULL,       -- Para conferência rápida
    nome_arquivo_gerado VARCHAR(255) NOT NULL,
    
    CONSTRAINT codaf_suplementar_log_remessa_conclusao_codaf_id_fk FOREIGN KEY (codaf_suplementar_id)
        REFERENCES public.codaf_suplementar (id)
);

-- Índice para verificar rapidamente se já foi gerado
CREATE INDEX IF NOT EXISTS idx_suplementar_log_remessa_codaf_id ON codaf_suplementar_log_remessa_conclusao(codaf_suplementar_id);