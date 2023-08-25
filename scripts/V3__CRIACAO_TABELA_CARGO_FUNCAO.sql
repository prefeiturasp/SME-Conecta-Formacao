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
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('AGPP/ASSIST.ADM. DE GEST O', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ANALISTA DE INF.CULT. E DESP. - BIBLIOTECA', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ANALISTA DE INF.CULT.E DESP. - ED.F SICA', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ANALISTA DE SA DE', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ASSESSOR T CNICO EDUCACIONAL', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ASSISTENTE DE DIRETOR DE ESCOLA', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ASSISTENTE T CNICO DE EDUCA  O I', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ASSISTENTE T CNICO EDUCACIONAL', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('AUXILIAR ADM. DE ENSINO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('AUXILIAR DE DESENVOLVIMENTO INFANTIL', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('AUXILIAR T CNICO DE EDUCA  O', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('BIBLIOTEC RIO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('COORDENADOR DE A  O CULTURAL', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('COORDENADOR DE A  O EDUCACIONAL', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('COORDENADOR DE ESPORTES E LAZER', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('COORDENADOR PEDAG GICO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('DIRETOR DE DIVIS O T CNICA', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('DIRETOR DE DIVIS O/CHEFE DE N CLEO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('DIRETOR DE ESCOLA', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('DIRETOR REGIONAL DE EDUCA  O', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('ESTUDANTE ESTAGI RIO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('GESTOR DE CENTRO EDUCACIONAL UNIFICADO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('NUTRICIONISTA', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROF. DE ED. INFANTIL', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROF. ED. INF. E ENS. FUND. I', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROF. ENS. FUND. II E M DIO', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('SECRET RIO DE ESCOLA', 1, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('SUPERVISOR ESCOLAR', 1, now(), 'Sistema', 'Sistema', false);
																		 
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('COORDENADOR DE POLO UNICEU', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROFESSOR DE APOIO E ACOMPANHAMENTO   INCLUS O - PAAI', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROFESSOR DE ATENDIMENTO EDUCACIONAL ESPECIALIZADO - PAEE', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROFESSOR DE APOIO PEDAG GICO - PAP', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROFESSOR ORIENTADOR DE  REA - POA', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROFESSOR ORIENTADOR DE EDUCA  O INTEGRAL - POEI', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROFESSOR ORIENTADOR DE EDUCA  O DIGITAL - POED', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('PROFESSOR ORIENTADOR DE SALA DE LEITURA - POSL', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('SERV. TEC. ADMINISTRATIVOS', 2, now(), 'Sistema', 'Sistema', false);
insert into cargo_funcao (nome, tipo, criado_em, criado_por, criado_login, excluido) values ('SERV. TEC. EDUCACIONAIS', 2, now(), 'Sistema', 'Sistema', false);