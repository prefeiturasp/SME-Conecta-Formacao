CREATE IF NOT EXISTS TABLE usuario_acessibilidade (
    id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
    usuario_id int8 NOT NULL,
    possui_deficiencia BIT NOT NULL,
    descricao_deficiencia VARCHAR(500) NULL,
    necessita_adaptacao BIT NULL,
    descricao_adaptacao VARCHAR(1000) NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,	
	CONSTRAINT usuario_acessibilidade_pk PRIMARY KEY (id),
	CONSTRAINT usu_acess_usuario_id_fk FOREIGN KEY (usuario_id) REFERENCES USUARIO(id)	
);

CREATE INDEX IF NOT EXISTS idx_usuario_acessibilidade_usuario_id ON usuario_acessibilidade (usuario_id);
CREATE UNIQUE INDEX IF NOT EXISTS idx_usuario_acessibilidade_usuario_id_unq ON usuario_acessibilidade (usuario_id) WHERE excluido = false;