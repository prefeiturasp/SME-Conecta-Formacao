--> Criação da tabela de anos das turmas
CREATE TABLE if not exists public.ano_turma (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	codigo_eol varchar(1) NULL,	
	descricao varchar(70) not null,
	codigo_serie_ensino int8 null,
	ano_letivo int2 null,
	modalidade int8 null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool default('false'),
	todos bool default('false'),
	ordem int2 default(1),
	CONSTRAINT ano_turma_pk PRIMARY KEY (id)	
);
CREATE INDEX if not exists ano_turma_codigo_serie_ensino_idx ON public.ano_turma (codigo_serie_ensino);
CREATE INDEX if not exists ano_turma_modalidade_idx ON public.ano_turma (modalidade);
CREATE INDEX if not exists ano_turma_codigo_eol_idx ON public.ano_turma (codigo_eol);

insert into public.ano_turma
(descricao,criado_em,criado_por,criado_login, ordem, ano_letivo)
select 'Todos',now(), 'Sistema', 'Sistema',0,date_part('year', CURRENT_DATE) where not exists (select 1 from public.ano_turma where descricao = 'Todos');
 
--> Criação da tabela de Componentes curriculares
CREATE TABLE if not exists public.componente_curricular (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	ano_turma_id int8 null,	
	codigo_eol int4 NULL,	
	nome varchar(70) not null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,	
	excluido bool default('false'),
	todos bool default('false'),
	ordem int2 default(1),
	CONSTRAINT componente_curricular_pk PRIMARY KEY (id),
	CONSTRAINT componente_curricular_ano_turma_id_fk FOREIGN KEY (ano_turma_id) REFERENCES public.ano_turma(id)
);
CREATE INDEX if not exists componente_curricular_codigo_eol_idx ON public.componente_curricular(codigo_eol);

insert into public.componente_curricular
(nome,criado_em,criado_por,criado_login, ordem)
select 'Todos',now(), 'Sistema', 'Sistema',0 where not exists (select 1 from public.componente_curricular where nome = 'Todos');