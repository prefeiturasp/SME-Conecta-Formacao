-- Criar tabela para rastreamento de e-mails enviados (idempotência)
CREATE TABLE IF NOT EXISTS public.email_enviado (
	id BIGSERIAL PRIMARY KEY,
	chave_idempotencia VARCHAR(255) NOT NULL,
	email_destinatario VARCHAR(255) NOT NULL,
	nome_destinatario VARCHAR(500) NULL,
	titulo VARCHAR(500) NOT NULL,
	conteudo_hash VARCHAR(255) NULL,
	enviado_em TIMESTAMP NOT NULL,
	notificacao_usuario_id BIGINT NULL,
	tentativas_envio INT NOT NULL DEFAULT 1,
	mensagem_erro TEXT NULL,
	criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	criado_por VARCHAR(200) NOT NULL DEFAULT 'Sistema',
	criado_rf VARCHAR(200) NOT NULL DEFAULT 'Sistema',
	alterado_em TIMESTAMP NULL,
	alterado_por VARCHAR(200) NULL,
	alterado_rf VARCHAR(200) NULL,
	excluido BOOLEAN NOT NULL DEFAULT FALSE,
	CONSTRAINT fk_email_enviado_notificacao_usuario FOREIGN KEY (notificacao_usuario_id) REFERENCES public.notificacao_usuario(id)
);

-- Índices para performance
CREATE UNIQUE INDEX IF NOT EXISTS idx_email_enviado_chave_idempotencia ON public.email_enviado(chave_idempotencia) WHERE NOT excluido;
CREATE INDEX IF NOT EXISTS idx_email_enviado_email_destinatario ON public.email_enviado(email_destinatario) WHERE NOT excluido;
CREATE INDEX IF NOT EXISTS idx_email_enviado_notificacao_usuario_id ON public.email_enviado(notificacao_usuario_id) WHERE NOT excluido;
CREATE INDEX IF NOT EXISTS idx_email_enviado_enviado_em ON public.email_enviado(enviado_em DESC);

-- Comentários de documentação
COMMENT ON TABLE public.email_enviado IS 'Registra histórico de e-mails enviados para implementação de idempotência e auditoria';
COMMENT ON COLUMN public.email_enviado.chave_idempotencia IS 'Chave única SHA256 para garantir que o mesmo e-mail não seja enviado mais de uma vez';
COMMENT ON COLUMN public.email_enviado.conteudo_hash IS 'Hash SHA256 do conteúdo do e-mail para detectar duplicatas de conteúdo';
COMMENT ON COLUMN public.email_enviado.notificacao_usuario_id IS 'FK opcional para vincular e-mails enviados via notificação';
COMMENT ON COLUMN public.email_enviado.tentativas_envio IS 'Contador de tentativas de envio (sucesso ou falha)';
COMMENT ON COLUMN public.email_enviado.mensagem_erro IS 'Mensagem de erro da última tentativa de envio (NULL se bem-sucedido)';

-- Adicionar colunas de rastreamento de envio de e-mail na tabela notificacao_usuario
ALTER TABLE public.notificacao_usuario
ADD COLUMN IF NOT EXISTS email_enviado_em TIMESTAMP NULL,
ADD COLUMN IF NOT EXISTS email_enviado BOOLEAN NOT NULL DEFAULT FALSE,
ADD COLUMN IF NOT EXISTS email_hash VARCHAR(255) NULL,
ADD COLUMN IF NOT EXISTS tentativas_envio_email INT NOT NULL DEFAULT 0,
ADD COLUMN IF NOT EXISTS email_erro TEXT NULL;

-- Índices para otimizar consultas
CREATE INDEX IF NOT EXISTS idx_notificacao_usuario_email_enviado ON public.notificacao_usuario(email_enviado) WHERE NOT excluido;
CREATE INDEX IF NOT EXISTS idx_notificacao_usuario_email_hash ON public.notificacao_usuario(email_hash) WHERE NOT excluido AND email_hash IS NOT NULL;

-- Comentários de documentação
COMMENT ON COLUMN public.notificacao_usuario.email_enviado_em IS 'Data/hora em que o e-mail foi enviado com sucesso';
COMMENT ON COLUMN public.notificacao_usuario.email_enviado IS 'Flag indicando se o e-mail foi enviado com sucesso';
COMMENT ON COLUMN public.notificacao_usuario.email_hash IS 'Hash SHA256 da chave de idempotência do e-mail enviado';
COMMENT ON COLUMN public.notificacao_usuario.tentativas_envio_email IS 'Contador de tentativas de envio de e-mail (sucesso ou falha)';
COMMENT ON COLUMN public.notificacao_usuario.email_erro IS 'Mensagem de erro da última tentativa de envio de e-mail (NULL se bem-sucedido)';
