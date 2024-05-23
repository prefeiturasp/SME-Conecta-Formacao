CREATE TABLE IF NOT EXISTS public.notificacao (
	id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
	titulo varchar(200) NOT NULL,
	mensagem varchar NOT NULL,	
	categoria int2 NOT NULL,
	tipo int2 NOT NULL,
	parametros text NOT NULL,
	excluida bool NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_rf varchar(200) NOT NULL,
	alterado_rf varchar(200) NULL,
	CONSTRAINT notificacao_pk PRIMARY KEY (id)
);
CREATE INDEX idx_notificacao_id ON public.notificacao USING btree (id);

CREATE TABLE IF NOT EXISTS public.notificacao_usuario (
	id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
	notificacao_id int8 not null,
	usuario_id int8 not null,
	status int2 NOT NULL,
	excluida bool NOT NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_rf varchar(200) NOT NULL,
	alterado_rf varchar(200) NULL,
	CONSTRAINT nnotificacao_usuario_pk PRIMARY KEY (id),
	CONSTRAINT notificacao_usuario_usuario_fk FOREIGN KEY (usuario_id) REFERENCES public.usuario(id),
	CONSTRAINT notificacao_usuario_notificacao_fk FOREIGN KEY (notificacao_id) REFERENCES public.notificacao(id)
);
CREATE INDEX idx_notificacao_usuario_id ON public.notificacao_usuario USING btree (id);
CREATE INDEX idx_notificacao_usuario_usuario ON public.notificacao_usuario USING btree (usuario_id);