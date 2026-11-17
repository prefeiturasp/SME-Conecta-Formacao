CREATE TABLE IF NOT EXISTS codaf_conferencia(
    id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
    proposta_id int8 NOT NULL,
    proposta_turma_id int8 NOT NULL,
    numero_comunicado SMALLINT NULL,
    data_comunicado timestamp without time zone NULL,
    pagina_comunicado_dom SMALLINT NULL,
    data_publicacao_dom timestamp without time zone NULL,
    codigo_curso_eol int NULL,
    codigo_nivel int NULL,
    observacao text NULL,
    status int NOT NULL,
    data_finalizacao timestamptz NULL,
    criado_em timestamp NOT NULL,
    criado_por varchar(200) NOT NULL,
    alterado_em timestamp NULL,
    alterado_por varchar(200) NULL,
    criado_login varchar(200) NOT NULL,
    alterado_login varchar(200) NULL,
    excluido bool NOT NULL,

    CONSTRAINT codaf_conferencia_pk PRIMARY KEY (id),
    CONSTRAINT codaf_conferencia_prop_id_fk FOREIGN KEY (proposta_id) REFERENCES PROPOSTA(id),
    CONSTRAINT codaf_conferencia_prop_turma_id_fk FOREIGN KEY (proposta_turma_id) REFERENCES PROPOSTA_TURMA(id)
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_codaf_conferencia_proposta_turma_id_ativo
ON codaf_conferencia (proposta_turma_id)
WHERE excluido = false;

CREATE TABLE IF NOT EXISTS codaf_conferencia_inscricao(
    id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
    codaf_conferencia_id int8 NOT NULL,
    inscricao_id int8 NOT NULL,
    participou boolean NULL,
    criado_em timestamp NOT NULL,
    criado_por varchar(200) NOT NULL,
    alterado_em timestamp NULL,
    alterado_por varchar(200) NULL,
    criado_login varchar(200) NOT NULL,
    alterado_login varchar(200) NULL,
    excluido bool NOT NULL,

    CONSTRAINT codaf_conferencia_inscricao_pk PRIMARY KEY (id),
    CONSTRAINT codaf_conf_insc_codaf_conf_id_fk FOREIGN KEY (codaf_conferencia_id) REFERENCES CODAF_CONFERENCIA(id),
    CONSTRAINT codaf_conf_insc_insc_id_fk FOREIGN KEY (inscricao_id) REFERENCES INSCRICAO(id),
    CONSTRAINT codaf_conf_insc_codaf_id_insc_id_key UNIQUE(codaf_conferencia_id, inscricao_id)
);

CREATE TABLE IF NOT EXISTS codaf_conferencia_anexo(
    id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
    codaf_conferencia_id int8 NOT NULL,
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
    excluido bool NOT NULL,

    CONSTRAINT codaf_conferencia_anexo_pk PRIMARY KEY (id),
    CONSTRAINT codaf_conf_anexo_codaf_conf_id_fk FOREIGN KEY (codaf_conferencia_id) REFERENCES CODAF_CONFERENCIA(id)
);
