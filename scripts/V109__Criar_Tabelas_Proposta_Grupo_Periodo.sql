CREATE TABLE IF NOT EXISTS proposta_grupo_periodo (
    id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
    proposta_id int8 NOT NULL,
    descricao varchar(255) NULL,
    data_inicio timestamp NOT NULL,
    data_fim timestamp NOT NULL,
    
    -- Campos de auditoria
    criado_em timestamp NOT NULL,
    criado_por varchar(200) NOT NULL,
    alterado_em timestamp NULL,
    alterado_por varchar(200) NULL,
    criado_login varchar(200) NOT NULL,
    alterado_login varchar(200) NULL,
    excluido boolean NOT NULL DEFAULT false,
    
    CONSTRAINT proposta_grupo_periodo_pk PRIMARY KEY (id),
    CONSTRAINT proposta_grupo_periodo_proposta_fk FOREIGN KEY (proposta_id) REFERENCES proposta (id)
);

CREATE TABLE IF NOT EXISTS proposta_grupo_periodo_turma (
    grupo_periodo_id int8 NOT NULL,
    proposta_turma_id int8 NOT NULL,
    
    -- Campos de auditoria
    criado_em timestamp NOT NULL,
    criado_por varchar(200) NOT NULL,
    alterado_em timestamp NULL,
    alterado_por varchar(200) NULL,
    criado_login varchar(200) NOT NULL,
    alterado_login varchar(200) NULL,
    excluido boolean NOT NULL DEFAULT false,
    
    CONSTRAINT proposta_grupo_periodo_turma_pk PRIMARY KEY (grupo_periodo_id, proposta_turma_id),
    CONSTRAINT pgpt_grupo_periodo_fk FOREIGN KEY (grupo_periodo_id) REFERENCES proposta_grupo_periodo (id),
    CONSTRAINT pgpt_turma_fk FOREIGN KEY (proposta_turma_id) REFERENCES proposta_turma (id)
);

CREATE INDEX IF NOT EXISTS ix_proposta_grupo_periodo_proposta_id ON proposta_grupo_periodo (proposta_id);
CREATE INDEX IF NOT EXISTS ix_proposta_grupo_periodo_turma_proposta_turma_id ON proposta_grupo_periodo_turma (proposta_turma_id);