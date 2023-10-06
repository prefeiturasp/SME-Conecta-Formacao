create table if not exists public.parametro_sistema (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE),
	nome varchar(50) NOT NULL,
	tipo int4 NOT NULL,
	descricao varchar(200) NOT NULL,
	valor varchar(100) NOT NULL,
	ano int4 NULL,
	ativo bool NOT NULL DEFAULT true,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_rf varchar(200) NULL,
	CONSTRAINT parametro_sistema_pk PRIMARY KEY (id)
);
CREATE INDEX parametro_sistema_ano_idx ON public.parametro_sistema USING btree (ano);
CREATE INDEX parametro_sistema_tipo_idx ON public.parametro_sistema USING btree (tipo);

INSERT INTO public.parametro_sistema (nome, tipo, descricao, valor, ano, ativo, criado_em, criado_por, criado_login)
select 'ComunicadoAcaoFormativaDescricao', 
       1,
       'Texto da declaração da ação formativa em conformidade com o Comunicado',
       'Declaro a ação formativa está em conformidade com o Comunicado nº1.043, de 16 de dezembro de 2020',
       2023,
       true,
       now(),
       'Sistema',
       'Sistema'
where not exists (select 1 from public.parametro_sistema where nome = 'ComunicadoAcaoFormativaDescricao' and tipo = 1);

INSERT INTO public.parametro_sistema (nome, tipo, descricao, valor, ano, ativo, criado_em, criado_por, criado_login)
select 'ComunicadoAcaoFormativaUrl', 
		2,
		'Url da declaração da ação formativa em conformidade com o Comunicado',
		'https://educacao.sme.prefeitura.sp.gov.br/',
		2023,
		true,
		now(),
		'Sistema',
		'Sistema'
where not exists (select 1 from public.parametro_sistema where nome = 'ComunicadoAcaoFormativaUrl' and tipo = 2);