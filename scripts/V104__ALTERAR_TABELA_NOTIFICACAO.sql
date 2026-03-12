ALTER TABLE NOTIFICACAO
ADD COLUMN IF NOT EXISTS data_expiracao timestamptz NULL,
ADD COLUMN IF NOT EXISTS mensagem_apos_expiracao varchar NULL;