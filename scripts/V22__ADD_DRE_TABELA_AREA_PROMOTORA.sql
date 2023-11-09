ALTER TABLE public.area_promotora ADD dreid int8 NULL;
ALTER TABLE public.area_promotora ADD CONSTRAINT area_promotora_dre_fk FOREIGN KEY (dreid) REFERENCES dre(id);
CREATE INDEX area_promotora_dre_idx ON public.area_promotora USING btree (dreid);