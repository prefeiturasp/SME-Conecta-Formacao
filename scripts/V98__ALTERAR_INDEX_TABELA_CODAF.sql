DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'proposta_turma_id_key') THEN
        ALTER TABLE CODAF_LISTA_PRESENCA DROP CONSTRAINT proposta_turma_id_key;
    END IF;
END $$;

CREATE UNIQUE INDEX IF NOT EXISTS idx_proposta_turma_id_ativo
ON CODAF_LISTA_PRESENCA (proposta_turma_id)
WHERE excluido = false;