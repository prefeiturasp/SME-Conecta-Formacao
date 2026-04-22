ALTER TABLE proposta_encontro_data
ALTER COLUMN data_fim DROP NOT NULL,
ADD COLUMN IF NOT EXISTS hora_inicio varchar(5) null,
ADD COLUMN IF NOT EXISTS hora_fim varchar(5) null