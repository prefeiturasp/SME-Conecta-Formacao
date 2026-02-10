CREATE SEQUENCE IF NOT EXISTS public.seq_certificados_numero
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

/* Criação da Tabela de Certificados */
CREATE TABLE IF NOT EXISTS public.codaf_certificados (
    id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
    codigo_certificado BIGINT NOT NULL DEFAULT nextval('public.seq_certificados_numero'),
    codaf_inscricao_lista_presenca_id int8 NULL,
    proposta_regente_turma_id int8 NULL,
    tipo_participacao INT NOT NULL, -- 1: Cursista, 2: Regente/Instrutor
    data_emissao TIMESTAMP NOT NULL DEFAULT NOW(),
    html_content_snapshot TEXT NOT NULL, -- Compliance: armazena o HTML exato gerado
    metadados_json JSONB NULL, -- Flexibilidade para dados extras (Nome Formação, Carga Horária, etc)
    status_processamento INT NOT NULL DEFAULT 1, -- 1: Pendente, 2: Processando, 3: Emitido, 4: Erro
    chave_objeto_armazenamento VARCHAR(255) NULL, -- Onde o certificado gerado está salvo (S3, etc)
    erro_processamento TEXT NULL,
    tentativas_processamento INT NOT NULL DEFAULT 0,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,

    CONSTRAINT codaf_certificados_pk PRIMARY KEY (id),
    CONSTRAINT codaf_certificados_inscricao_fk FOREIGN KEY (codaf_inscricao_lista_presenca_id)
        REFERENCES public.codaf_inscricao_lista_presenca (id),
    CONSTRAINT codaf_certificados_regente_fk FOREIGN KEY (proposta_regente_turma_id)
        REFERENCES public.proposta_regente_turma (id)
);

/* Índices para performance */
CREATE UNIQUE INDEX IF NOT EXISTS idx_codaf_certificados_codigo ON public.codaf_certificados (codigo_certificado);
CREATE INDEX IF NOT EXISTS idx_codaf_certificados_inscricao ON public.codaf_certificados (codaf_inscricao_lista_presenca_id);
CREATE INDEX IF NOT EXISTS idx_codaf_certificados_data_emissao ON public.codaf_certificados (data_emissao);
CREATE INDEX IF NOT EXISTS idx_codaf_certificados_status ON public.codaf_certificados (status_processamento);

/* Comentários da Tabela */
COMMENT ON TABLE public.codaf_certificados IS 'Armazena os certificados emitidos para cursistas e regentes da SME.';
COMMENT ON COLUMN public.codaf_certificados.html_content_snapshot IS 'Snapshot do HTML gerado no momento da emissão para fins de auditoria e reimpressão fidedigna.';