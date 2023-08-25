CREATE TABLE IF NOT EXISTS public.criterio_validacao_inscricao (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( NO MINVALUE NO MAXVALUE NO CYCLE),
	nome varchar(200) NULL,
	unico boolean NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT NULL,
	CONSTRAINT criterio_validacao_inscricao_pk PRIMARY KEY (id)
);

CREATE index if not exists criterio_validacao_inscricao_excluido_idx ON public.criterio_validacao_inscricao (excluido);

insert into criterio_validacao_inscricao (nome, unico, criado_em, criado_por, criado_login, excluido) values ('Não há critérios para validação das inscrições', true, now(), 'Sistema', 'Sistema', false);
insert into criterio_validacao_inscricao (nome, unico, criado_em, criado_por, criado_login, excluido) values ('Será priorizada a inscrição de um servidor por Unidade Educacional', false, now(), 'Sistema', 'Sistema', false);
insert into criterio_validacao_inscricao (nome, unico, criado_em, criado_por, criado_login, excluido) values ('As inscrições serão validadas pela ordem de cadastro no link, considerando as especificações do público-alvo', false, now(), 'Sistema', 'Sistema', false);
insert into criterio_validacao_inscricao (nome, unico, criado_em, criado_por, criado_login, excluido) values ('Será priorizada a inscrição de quem não realizou a formação em outras edições', false, now(), 'Sistema', 'Sistema', false);
insert into criterio_validacao_inscricao (nome, unico, criado_em, criado_por, criado_login, excluido) values ('Não terá prioridade o servidor que desistiu de formações anteriores sem justificativa', false, now(), 'Sistema', 'Sistema', false);