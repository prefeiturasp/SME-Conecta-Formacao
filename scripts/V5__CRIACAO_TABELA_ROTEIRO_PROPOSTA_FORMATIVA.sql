CREATE TABLE IF NOT EXISTS public.roteiro_proposta_formativa (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( NO MINVALUE NO MAXVALUE NO CYCLE),
	descricao varchar(200) NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT NULL,
	CONSTRAINT roteiro_proposta_formativa_pk PRIMARY KEY (id)
);

CREATE index if not exists roteiro_proposta_formativa_excluido_idx ON public.usuario (excluido);

insert into roteiro_proposta_formativa (descricao, criado_em, criado_por, criado_login, excluido) values ('O título da formação deve apresentar de forma sucinta a idéia central do tema que será tratado, indicando ao cursista o macro área do tema e a especificidade do curso proposto.', now(), 'Sistema', 'Sistema', false);