CREATE TABLE if not exists public.inscricao (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_turma_id int8 not NULL,
	usuario_id int8 not null,
	codigo_cargo_eol int8 null,
	cargo_eol varchar(50) null,
	codigo_tipo_funcao_eol int8 null,
	tipo_funcao_eol varchar(50) null,
	arquivo_id int8 null,
	situacao smallint not null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	criado_login varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL DEFAULT false,
	CONSTRAINT inscricao_pk PRIMARY KEY (id),
	CONSTRAINT inscricao_proposta_turma_fk FOREIGN KEY (proposta_turma_id) REFERENCES proposta_turma (id),
	CONSTRAINT inscricao_usuario_fk FOREIGN KEY (usuario_id) REFERENCES public.usuario(id)
);

alter table usuario add if not exists cpf varchar(11);