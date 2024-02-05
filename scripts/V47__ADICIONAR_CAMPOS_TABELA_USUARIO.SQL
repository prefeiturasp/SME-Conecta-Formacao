ALTER TABLE public.usuario ADD ue_codigo varchar(20) NULL;
ALTER TABLE public.usuario ADD tipo int4 NULL DEFAULT 1;
ALTER TABLE public.usuario ADD possui_contrato_externo boolean NULL DEFAULT false;
ALTER TABLE public.usuario ADD situacao_cadastro int4 NULL DEFAULT 1;
