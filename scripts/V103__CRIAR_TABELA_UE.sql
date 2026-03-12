CREATE TABLE IF NOT EXISTS ue(
	id uuid,
	dre_id bigint NOT NULL,
	codigo_ue char(6) NOT NULL,
	tipo_escola int NOT NULL,
	sigla_tipo_escola char(12) NOT NULL,
	nome_escola varchar(60) NOT NULL,
	data_atualizacao TIMESTAMP WITH time ZONE DEFAULT now(),
	CONSTRAINT ue_pk PRIMARY KEY (id),
	CONSTRAINT ue_dre_id_fk FOREIGN KEY (dre_id) REFERENCES DRE(id)
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_ue_key 
ON ue (codigo_ue);