CREATE TABLE codaf_anexo (
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
CREATE INDEX IX_codaf_anexo_codaf_lista_presenca_id ON codaf_anexo(codaf_lista_presenca_id) WHERE excluido IS FALSE;