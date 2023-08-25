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

insert into criterio_validacao_inscricao (nome, unico, criado_em, criado_por, criado_login, excluido) values ('N�o h� crit�rios para valida��o das inscri��es', true, now(), 'Sistema', 'Sistema', false);
insert into criterio_validacao_inscricao (nome, unico, criado_em, criado_por, criado_login, excluido) values ('Ser� priorizada a inscri��o de um servidor por Unidade Educacional', false, now(), 'Sistema', 'Sistema', false);
insert into criterio_validacao_inscricao (nome, unico, criado_em, criado_por, criado_login, excluido) values ('As inscri��es ser�o validadas pela ordem de cadastro no link, considerando as especifica��es do p�blico-alvo', false, now(), 'Sistema', 'Sistema', false);
insert into criterio_validacao_inscricao (nome, unico, criado_em, criado_por, criado_login, excluido) values ('Ser� priorizada a inscri��o de quem n�o realizou a forma��o em outras edi��es', false, now(), 'Sistema', 'Sistema', false);
insert into criterio_validacao_inscricao (nome, unico, criado_em, criado_por, criado_login, excluido) values ('N�o ter� prioridade o servidor que desistiu de forma��es anteriores sem justificativa', false, now(), 'Sistema', 'Sistema', false);