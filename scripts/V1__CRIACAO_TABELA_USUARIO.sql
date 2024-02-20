CREATE TABLE IF NOT EXISTS public.usuario (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY( NO MINVALUE NO MAXVALUE NO CYCLE),
	login varchar(50) NULL,
	nome varchar(100) NULL,
	ultimo_login timestamp NULL,
	expiracao_recuperacao_senha timestamp NULL,
	token_recuperacao_senha uuid NULL,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT NULL,
	CONSTRAINT usuario_pk PRIMARY KEY (id),
	CONSTRAINT usuario_un_login UNIQUE (login)
);

CREATE index if not exists usuario_login_idx ON public.usuario (login);