CREATE TABLE IF NOT EXISTS funcoes_atividades_eol(
	id uuid PRIMARY KEY,
	cd_cargo_base_cotic int NOT NULL,
	cd_registro_funcional char(7) NOT NULL,
	cd_tipo_funcao int4 NOT NULL,
	nome_funcao varchar(200) NULL,
	tipo_vinculo INT NULL,
	codigo_dre char(6) NOT NULL,
	codigo_ue char(6) NOT NULL,
	data_posse DATE NULL,
	data_atualizacao TIMESTAMP WITH time ZONE DEFAULT now()
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_funcoes_atividades_eol_key
ON funcoes_atividades_eol (
    cd_registro_funcional,
    cd_tipo_funcao,
    codigo_ue
);

CREATE INDEX IF NOT EXISTS ix_funcoes_atividades_eol_ue
ON funcoes_atividades_eol (codigo_ue);

CREATE INDEX IF NOT EXISTS ix_funcoes_atividades_eol_registro_funcional
ON funcoes_atividades_eol (cd_registro_funcional);

CREATE INDEX IF NOT EXISTS ix_funcoes_atividades_eol_dre
ON funcoes_atividades_eol (codigo_dre);