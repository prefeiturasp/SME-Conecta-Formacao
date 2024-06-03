--> Adicionar campo que indica se critério permite sorteio de inscrições
ALTER TABLE public.criterio_validacao_inscricao ADD IF NOT EXISTS permite_sorteio bool DEFAULT false NOT NULL;

--> Definir critério como permite sorteio              
update criterio_validacao_inscricao 
set permite_sorteio = true 
where nome = 'Caso o número de inscritos ultrapasse o número de vagas será realizado sorteio';

