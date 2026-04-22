ALTER TABLE NOTIFICACAO
ADD COLUMN IF NOT EXISTS correlacao_id uuid NULL;

CREATE INDEX IF NOT EXISTS idx_notificacao_correlacao_id
ON notificacao(correlacao_id);