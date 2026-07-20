UPDATE PUBLIC.CARGO_FUNCAO
SET    ordem = ordem + 1
WHERE  ORDEM BETWEEN 25 AND 999;


INSERT INTO cargo_funcao (
    nome,
    tipo,
    criado_em,
    criado_por,
    criado_login,
    excluido,
    ordem
)
SELECT
    'FUNCIONÁRIO DE UNIDADE PARCEIRA',
    1,
    NOW(),
    'Sistema',
    'Sistema',
    FALSE,
    25
WHERE NOT EXISTS (
    SELECT 1
    FROM cargo_funcao
    WHERE UPPER(BTRIM(nome)) =
          UPPER(BTRIM('FUNCIONÁRIO DE UNIDADE PARCEIRA'))
);