--> Adicionando campo 'grupo_gestao_id' na 'proposta'
ALTER TABLE public.proposta add column if not exists grupo_gestao_id int8 NULL;

--> Adicionando índice do campo 'grupo_gestao_id'
CREATE INDEX if not exists proposta_grupo_gestao_id_idx ON public.proposta USING btree (grupo_gestao_id);

--> Adicionando constraint do campo 'grupo_gestao_id'
ALTER TABLE public.proposta drop CONSTRAINT if exists proposta_grupo_gestao_fk;
ALTER TABLE public.proposta add CONSTRAINT proposta_grupo_gestao_fk FOREIGN KEY (grupo_gestao_id) REFERENCES public.grupo_gestao(id);