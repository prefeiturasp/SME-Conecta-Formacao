update public.proposta set formacao_homologada = null where formacao_homologada is not null;

ALTER TABLE public.proposta ALTER COLUMN formacao_homologada TYPE integer USING formacao_homologada::integer;