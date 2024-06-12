--> Dropar tabela antiga
drop table if exists proposta_parecer;

--> Criação da tabela de proposta_parecerista_consideracao
CREATE TABLE if not exists public.proposta_parecerista_consideracao (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_parecerista_id int8 NOT NULL,
	campo int2 NOT NULL,	
	descricao varchar(1000) NOT NULL,	
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL,
	CONSTRAINT proposta_parecerista_consideracao_pk PRIMARY KEY (id),
	CONSTRAINT proposta_parecerista_consideracao_proposta_parecerista_fk FOREIGN KEY (proposta_parecerista_id) REFERENCES public.proposta_parecerista(id)
);

--> Criando índice de proposta_parecerista_id em proposta_parecerista_consideracao
CREATE INDEX if not exists proposta_parecerista_consideracao_proposta_parecerista_idx ON public.proposta_parecerista_consideracao (proposta_parecerista_id);

--> Parâmetro de QtdeLimitePareceristaProposta
insert into parametro_sistema (nome, tipo, descricao, valor, ano, ativo, criado_em, criado_por, criado_login)
select 'QtdeLimitePareceristaProposta', 7, 'Estabelece a quantidade máxima de pareceristas que podem ser inseridos na proposta para dar parecer','3', 2024, true, now(), 'Sistema', 'Sistema' 
where not exists (select id from parametro_sistema where ano = 2024 and tipo = 7);

--> Alterar proposta_parecerista

alter table proposta_parecerista add if not exists  situacao int2 null;
alter table proposta_parecerista add if not exists  usuario_id int2 null;
alter table proposta_parecerista add if not exists  justificativa text null;

--> Constraints 
ALTER TABLE proposta_parecerista DROP CONSTRAINT IF EXISTS proposta_parecerista_usuario_fk;
alter table proposta_parecerista add constraint proposta_parecerista_usuario_fk FOREIGN KEY (usuario_id) REFERENCES usuario(id);