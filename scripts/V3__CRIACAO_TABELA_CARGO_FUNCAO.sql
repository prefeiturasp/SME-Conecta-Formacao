CREATE TABLE IF NOT EXISTS public.cargo_funcao (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	nome varchar(100) NOT NULL,
	tipo smallint NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT NULL,
	CONSTRAINT cargo_funcao_pk PRIMARY KEY (id)
);

CREATE index if not exists cargo_funcao_tipo_idx ON public.cargo_funcao (tipo);
CREATE index if not exists cargo_funcao_excluido_idx ON public.cargo_funcao (excluido);

insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('AGENTE DE APOIO/ASSIST. DE SUPORTE OPERACIONAL', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('AGENTE ESCOLAR', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('AGPP/ASSIST.ADM. DE GESTÃO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ANALISTA DE INF.CULT. E DESP. - BIBLIOTECA', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ANALISTA DE INF.CULT.E DESP. - ED.FÍSICA', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ANALISTA DE SAÚDE', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ASSESSOR TÉCNICO EDUCACIONAL', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ASSISTENTE DE DIRETOR DE ESCOLA', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ASSISTENTE TÉCNICO DE EDUCAÇÃO I', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ASSISTENTE TÉCNICO EDUCACIONAL', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('AUXILIAR ADM. DE ENSINO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('AUXILIAR DE DESENVOLVIMENTO INFANTIL', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('AUXILIAR TÉCNICO DE EDUCAÇÃO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('BIBLIOTECÁRIO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('COORDENADOR DE AÇÃO CULTURAL', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('COORDENADOR DE AÇÃO EDUCACIONAL', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('COORDENADOR DE ESPORTES E LAZER', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('COORDENADOR PEDAGÓGICO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('DIRETOR DE DIVISÃO TÉCNICA', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('DIRETOR DE DIVISÃO/CHEFE DE NÚCLEO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('DIRETOR DE ESCOLA', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('DIRETOR REGIONAL DE EDUCAÇÃO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ESTUDANTE ESTAGIÁRIO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('GESTOR DE CENTRO EDUCACIONAL UNIFICADO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('NUTRICIONISTA', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROF. DE ED. INFANTIL', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROF. ED. INF. E ENS. FUND. I', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROF. ENS. FUND. II E MÉDIO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('SECRETÁRIO DE ESCOLA', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('SUPERVISOR ESCOLAR', 1, now(), 'Sistema', 'Sistema', false);

insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('COORDENADOR DE POLO UNICEU', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROFESSOR DE APOIO E ACOMPANHAMENTO À INCLUSÃO - PAAI', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROFESSOR DE ATENDIMENTO EDUCACIONAL ESPECIALIZADO - PAEE', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROFESSOR DE APOIO PEDAGÓGICO - PAP', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROFESSOR ORIENTADOR DE ÁREA - POA', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROFESSOR ORIENTADOR DE EDUCAÇÃO INTEGRAL - POEI', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROFESSOR ORIENTADOR DE EDUCAÇÃO DIGITAL - POED', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROFESSOR ORIENTADOR DE SALA DE LEITURA - POSL', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('SERV. TEC. ADMINISTRATIVOS', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('SERV. TEC. EDUCACIONAIS', 2, now(), 'Sistema', 'Sistema', false);