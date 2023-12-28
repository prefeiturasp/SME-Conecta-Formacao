CREATE TABLE if not exists public.inscricao (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_turma_id int8 not NULL,
	usuario_id int8 not null,
	cargo_id int8 null,
	cargo_codigo int8 null,
	cargo_dre_codigo int8 null,
	cargo_ue_codigo int8 null,	
	funcao_id int8 null,
	funcao_codigo int8 null,
	funcao_dre_codigo int8 null,
	funcao_ue_codigo int8 null,
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
	CONSTRAINT inscricao_usuario_fk FOREIGN KEY (usuario_id) REFERENCES public.usuario (id),
	CONSTRAINT inscricao_cargo_fk FOREIGN KEY (cargo_id) REFERENCES public.cargo_funcao (id),
	CONSTRAINT inscricao_funcao_fk FOREIGN KEY (funcao_id) REFERENCES public.cargo_funcao (id)
);

alter table usuario add if not exists cpf varchar(11);

CREATE TABLE if not exists public.proposta_turma_vaga (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_turma_id int8 not NULL,
	inscricao_id int8 null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	criado_login varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL DEFAULT false,
	CONSTRAINT proposta_turma_vaga_pk PRIMARY KEY (id),
	CONSTRAINT proposta_turma_vaga_turma_fk FOREIGN KEY (proposta_turma_id) REFERENCES proposta_turma (id),
	CONSTRAINT proposta_turma_vaga_inscricao_fk FOREIGN KEY (inscricao_id) REFERENCES public.inscricao (id)
);