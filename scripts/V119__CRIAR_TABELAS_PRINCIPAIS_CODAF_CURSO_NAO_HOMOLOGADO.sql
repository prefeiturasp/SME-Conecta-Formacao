CREATE TABLE IF NOT EXISTS codaf_curso_nao_homologado(
    id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
    proposta_id int8 NOT NULL,
	proposta_turma_id int8 NOT NULL,
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
    
    CONSTRAINT ccnh_pk PRIMARY KEY (id),
    CONSTRAINT ccnh_prop_id_fk FOREIGN KEY (proposta_id) REFERENCES PROPOSTA(id),
    CONSTRAINT ccnh_prop_turma_id_fk FOREIGN KEY (proposta_turma_id) REFERENCES PROPOSTA_TURMA(id),
    CONSTRAINT ccnh_prop_turma_id_key UNIQUE (proposta_turma_id)
);

CREATE TABLE IF NOT EXISTS codaf_curso_nao_homologado_inscricao(
    id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
    codaf_curso_nao_hom_id int8 NOT NULL,
    inscricao_id int8 NOT NULL,
    participou boolean NOT NULL,
    criado_em timestamp NOT NULL,
    criado_por varchar(200) NOT NULL,
    alterado_em timestamp NULL,
    alterado_por varchar(200) NULL,
    criado_login varchar(200) NOT NULL,
    alterado_login varchar(200) NULL,
    excluido bool NOT NULL,
    
    CONSTRAINT ccnh_inscricao_pk PRIMARY KEY (id),
    CONSTRAINT ccnh_insc_ccnh_id_fk FOREIGN KEY (codaf_curso_nao_hom_id) REFERENCES CODAF_CURSO_NAO_HOMOLOGADO(id),
    CONSTRAINT ccnh_insc_insc_id_fk FOREIGN KEY (inscricao_id) REFERENCES INSCRICAO(id),
    CONSTRAINT ccnh_insc_ccnh_id_insc_id_key UNIQUE(codaf_curso_nao_hom_id, inscricao_id)
);

CREATE TABLE IF NOT EXISTS codaf_curso_nao_homologado_anexo(
    id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
    codaf_curso_nao_hom_id int8 NOT NULL,
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
    
    CONSTRAINT ccnh_anexo_pk PRIMARY KEY (id),
    CONSTRAINT ccnh_anexo_ccnh_id_fk FOREIGN KEY (codaf_curso_nao_hom_id) REFERENCES CODAF_CURSO_NAO_HOMOLOGADO(id)
);