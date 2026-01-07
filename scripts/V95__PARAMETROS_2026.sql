--> Inserindo parâmetros de 2025 em 2026
INSERT INTO parametro_sistema (nome, tipo, descricao, valor, ano, ativo, criado_em, criado_por, criado_login)
SELECT nome, tipo, descricao, valor, 2026, ativo, criado_em, criado_por, criado_login
FROM parametro_sistema
WHERE ano = 2025
AND NOT EXISTS (SELECT id
FROM parametro_sistema
WHERE ano = 2026);