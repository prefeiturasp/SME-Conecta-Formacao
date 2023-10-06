ALTER TABLE public.proposta_criterio_certificacao ADD CONSTRAINT proposta_criterio_certificacao_proposta_fk FOREIGN KEY (proposta_id) references public.proposta(id);
ALTER TABLE public.proposta_criterio_certificacao ADD CONSTRAINT proposta_criterio_certificacao_criterio_certificacao_fk FOREIGN KEY (criterio_certificacao_id) references criterio_certificacao(id);

CREATE index if not exists proposta_criterio_certificacao_proposta_idx ON public.proposta (id);
CREATE index if not exists proposta_criterio_certificacao_criterio_certificacao_idx ON public.criterio_certificacao (id);