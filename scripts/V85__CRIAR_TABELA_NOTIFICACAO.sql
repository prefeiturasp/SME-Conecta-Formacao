--> Adicionando tabela de notificações
CREATE TABLE IF NOT EXISTS public.notificacao (
	id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
	titulo varchar(200) NOT NULL,
	mensagem varchar NOT NULL,	
	categoria int2 NOT NULL,
	tipo int2 NOT NULL,
	parametros text NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	CONSTRAINT notificacao_pk PRIMARY KEY (id)
);
CREATE INDEX if not exists idx_notificacao_id ON public.notificacao USING btree (id);

--> Adicionando tabela de notificações do usuário
CREATE TABLE IF NOT EXISTS public.notificacao_usuario (
	id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
	notificacao_id int8 not null,
	login varchar(50) NULL,
	nome varchar(100) NOT NULL,
	email varchar(100) NULL,
	situacao int2 NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	CONSTRAINT notificacao_usuario_pk PRIMARY KEY (id),
	CONSTRAINT notificacao_usuario_notificacao_fk FOREIGN KEY (notificacao_id) REFERENCES public.notificacao(id)
);
CREATE INDEX if not exists idx_notificacao_usuario_id ON public.notificacao_usuario USING btree (id);

--> Adicionando parâmetro para endereço do sistema
insert into parametro_sistema (nome, tipo, descricao, valor, ano, ativo, criado_em, criado_por, criado_login)
select 'UrlConectaFormacao', 8, 'Endereço do Conecta Formação','https://dev-conectaformacao.sme.prefeitura.sp.gov.br/', 2024, true, now(), 'Sistema', 'Sistema' 
where not exists (select id from parametro_sistema where ano = 2024 and tipo = 8);