create table if not exists proposta_tipo_inscricao (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 not NULL,
	tipo_inscricao int2 not null,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	criado_login varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	alterado_login varchar(200) NULL,
	excluido bool NOT NULL DEFAULT false,
	CONSTRAINT proposta_tipo_inscricao_pk PRIMARY KEY (id),
	CONSTRAINT proposta_tipo_inscricao_proposta_fk FOREIGN KEY (proposta_id) REFERENCES proposta (id)
);

insert
	into
	proposta_tipo_inscricao 
	(proposta_id,
	tipo_inscricao,
	criado_em,
	criado_por,
	criado_login,
	alterado_em,
	alterado_por,
	alterado_login,
	excluido)
select
	id as proposta_id,
	tipo_inscricao,
	criado_em,
	criado_por,
	criado_login,
	alterado_em,
	alterado_por,
	alterado_login,
	excluido
from
	proposta p
where p.tipo_inscricao is not null 
  and not exists(select 1 from proposta_tipo_inscricao pti where pti.proposta_id = p.id and pti.tipo_inscricao = p.tipo_inscricao);
  
alter table proposta drop column tipo_inscricao;