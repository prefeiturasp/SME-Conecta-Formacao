ALTER TABLE CODAF_RETIFICACAO_LISTA_PRESENCA
ALTER COLUMN id TYPE int8,
ALTER COLUMN data_retificacao TYPE timestamp;

ALTER TABLE codaf_comentario_lista_presenca
ALTER COLUMN id TYPE int8;

ALTER TABLE notificacao
ADD COLUMN IF NOT EXISTS correlacao_id uuid NULL,
ADD COLUMN IF NOT EXISTS tipo_origem int NULL;

ALTER TABLE codaf_comentario_lista_presenca
ADD COLUMN IF NOT EXISTS notificacao_correlacao_id uuid NULL;