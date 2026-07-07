CREATE TABLE IF NOT EXISTS logs (
    id BIGSERIAL PRIMARY KEY,
    criado_por VARCHAR(50),
    criado_login VARCHAR(50),
    criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    entidade VARCHAR(200),
    nivel_log VARCHAR(50),
    mensagem TEXT,
    complemento TEXT,
    excluido bool
);