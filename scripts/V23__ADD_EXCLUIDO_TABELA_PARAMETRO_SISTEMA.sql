ALTER TABLE public.parametro_sistema ADD if not exists excluido boolean default ('false');
ALTER TABLE public.parametro_sistema RENAME COLUMN alterado_rf TO alterado_login;