INSERT INTO public.palavra_chave (
    nome,
    criado_em,
    criado_por,
    criado_login,
    excluido
)
SELECT 
    'TECNOLOGIAS PARA APRENDIZAGEM',
    NOW(),
    'SISTEMA',
    '0',
    FALSE
WHERE NOT EXISTS (
    SELECT 1 
    FROM public.palavra_chave 
    WHERE nome = 'TECNOLOGIAS PARA APRENDIZAGEM'
);