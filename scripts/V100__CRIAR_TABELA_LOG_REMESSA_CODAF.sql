-- Criação da tabela de log
CREATE TABLE IF NOT EXISTS codaf_log_remessa_conclusao (
    id SERIAL PRIMARY KEY,
    codaf_lista_presenca_id BIGINT NOT NULL,
    criado_login varchar(200) NOT NULL,              -- Quem gerou (Auditoria)
    data_geracao TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL,
    hash_arquivo VARCHAR(64) NOT NULL,       -- SHA256
    quantidade_registros INT NOT NULL,       -- Para conferência rápida
    nome_arquivo_gerado VARCHAR(255) NOT NULL,
    
    CONSTRAINT codaf_log_remessa_conclusao_codaf_id_fk FOREIGN KEY (codaf_lista_presenca_id)
        REFERENCES public.codaf_lista_presenca (id)
);

-- Índice para verificar rapidamente se já foi gerado
CREATE INDEX IF NOT EXISTS idx_log_remessa_codaf_id ON codaf_log_remessa_conclusao(codaf_lista_presenca_id);