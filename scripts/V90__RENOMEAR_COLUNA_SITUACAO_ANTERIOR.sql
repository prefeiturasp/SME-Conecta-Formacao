-- Renomear a coluna existente para seguir o padrão (se já existir)

ALTER TABLE inscricao RENAME COLUMN situacaoanterior TO situacao_anterior;