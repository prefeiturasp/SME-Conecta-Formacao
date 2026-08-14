CREATE SEQUENCE IF NOT EXISTS public.seq_declaracoes_numero
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER TABLE codaf_declaracoes
ALTER COLUMN codigo_declaracao TYPE BIGINT,
ALTER COLUMN codigo_declaracao SET DEFAULT nextval('public.seq_declaracoes_numero');

CREATE UNIQUE INDEX IF NOT EXISTS idx_codaf_declaracoes_codigo ON public.codaf_declaracoes (codigo_declaracoes);