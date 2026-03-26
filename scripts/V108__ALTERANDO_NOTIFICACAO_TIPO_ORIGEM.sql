ALTER TABLE NOTIFICACAO
ADD COLUMN IF NOT EXISTS tipo_origem int4 NULL;

CREATE INDEX IF NOT EXISTS idx_notificacao_tipo_origem
ON notificacao(tipo_origem);