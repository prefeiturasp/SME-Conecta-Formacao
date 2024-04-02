--Adicionar Tipo de Email
ALTER TABLE public.usuario ADD IF NOT exists tipo_email int4 NULL;

--Atualizar Usuario já existentes
update usuario set tipo_email = 1 where tipo_email isnull ;
