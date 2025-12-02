CREATE TABLE IF NOT EXISTS cargos_eol(
	id uuid PRIMARY KEY,
	cd_cargo int NOT NULL,
	cd_registro_funcional char(7) NOT NULL,
	codigo_dre char(6) NOT NULL,
	codigo_ue char(6) NOT NULL,
	sobreposto bool NOT NULL,
	data_atualizacao TIMESTAMP WITH time ZONE DEFAULT now()
);

CREATE TABLE IF NOT EXISTS Atribuicoes_Servidor_Eol(
	id uuid PRIMARY KEY,
	chave_negocio varchar(50) NOT NULL,
	cd_modalidade SMALLINT NOT NULL,
	ano_serie char(1) NOT NULL,
	cd_componente_curricular int NOT NULL,
	cd_registro_funcional char(7) NOT NULL,
	codigo_ue char(6) NOT NULL,
	data_atualizacao TIMESTAMP WITH time ZONE DEFAULT now()
);

CREATE TABLE IF NOT EXISTS funcaoatividade_eol(
	id uuid PRIMARY KEY,	
	cd_registro_funcional char(7) NOT NULL,
	cd_tipo_funcao varchar(50) NOT NULL,
	codigo_ue char(6) NOT NULL,
	data_atualizacao TIMESTAMP WITH time ZONE DEFAULT now()
);

CREATE UNIQUE INDEX ix_cargos_eol_key 
ON cargos_eol (cd_cargo, cd_registro_funcional, codigo_ue, sobreposto);

CREATE INDEX ix_cargos_eol_dre_id ON cargos_eol (codigo_dre);

CREATE UNIQUE INDEX ix_atribuicoes_servidor_eol_chave_negocio
ON Atribuicoes_Servidor_Eol (chave_negocio);

CREATE UNIQUE INDEX ix_funcaoatividade_eol_key
ON funcaoatividade_eol (
    cd_registro_funcional,
    cd_tipo_funcao,
    codigo_ue
);

CREATE INDEX ix_funcaoatividade_eol_ue
ON funcaoatividade_eol (codigo_ue);

CREATE INDEX ix_funcaoatividade_eol_registro_funcional
ON funcaoatividade_eol (cd_registro_funcional);