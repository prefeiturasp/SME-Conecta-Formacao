CREATE TABLE IF NOT EXISTS public.coordenadoria (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( NO MINVALUE NO MAXVALUE NO CYCLE),
	nome varchar(50) NOT NULL,
	sigla varchar(10) NULL,
	excluido boolean NOT NULL DEFAULT FALSE,
	criado_em timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	CONSTRAINT coordenadoria_pk PRIMARY KEY (id)
);

CREATE index if not exists coordenadoria_excluido_idx ON public.coordenadoria (excluido);

ALTER TABLE public.area_promotora 
ADD COLUMN IF NOT EXISTS coordenadoria_id int8 NULL;

ALTER TABLE public.area_promotora 
DROP CONSTRAINT IF EXISTS area_promotora_coordenadoria_fk;

ALTER TABLE public.area_promotora 
ADD CONSTRAINT area_promotora_coordenadoria_fk FOREIGN KEY (coordenadoria_id) REFERENCES public.coordenadoria (id);