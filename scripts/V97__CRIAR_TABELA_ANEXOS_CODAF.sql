CREATE TABLE IF NOT EXISTS codaf_anexo (
    id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
    codaf_lista_presenca_id int8 NOT NULL,    
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
	
	CONSTRAINT codaf_anexo_pk PRIMARY KEY (id),
    CONSTRAINT codaf_anexo_codaf_lista_presenca_id_fk FOREIGN KEY (codaf_lista_presenca_id) REFERENCES codaf_lista_presenca(id)
);

-- Índice para busca rápida pelos anexos de um CODAF
CREATE INDEX IF NOT EXISTS IX_codaf_anexo_codaf_lista_presenca_id ON codaf_anexo(codaf_lista_presenca_id) WHERE excluido IS FALSE;

CREATE TABLE IF NOT EXISTS codaf_movimentacao_lista_presenca(
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
    codaf_lista_presenca_id int8 NOT NULL,
	status_codaf_lista_presenca int NOT NULL,
	codaf_comentario_lista_presenca_id int8 NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	criado_login varchar(200) NOT NULL,
	
	CONSTRAINT codaf_mov_lp_pk PRIMARY KEY (id),
	CONSTRAINT codaf_mov_lp_codaf_lp_id_fk FOREIGN KEY (codaf_lista_presenca_id) REFERENCES codaf_lista_presenca,
	CONSTRAINT codaf_mov_lp_codaf_comentario_lp_id_fk FOREIGN KEY (codaf_comentario_lista_presenca_id) REFERENCES codaf_comentario_lista_presenca
);

CREATE INDEX IF NOT EXISTS ix_codaf_mov_lp_lista_id_data_status 
ON codaf_movimentacao_lista_presenca(codaf_lista_presenca_id, status_codaf_lista_presenca, criado_em DESC);