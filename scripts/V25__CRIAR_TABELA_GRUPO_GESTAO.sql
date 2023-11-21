CREATE TABLE if not exists public.grupo_gestao  (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	grupo_id uuid,
	nome varchar(200) NULL
);
CREATE INDEX if not exists grupo_gestao_id_idx ON public.grupo_gestao (id);

insert into grupo_gestao(grupo_id,nome)
select uuid_in('58E6A4FC-9588-EE11-97DC-00155DB4374A'),'Gestão DIEFEM' where not exists (select * from grupo_gestao where nome = 'Gestão DIEFEM') union all
select uuid_in('20B89885-9688-EE11-97DC-00155DB4374A'),'Gestão DIEE' where not exists (select * from grupo_gestao where nome = 'Gestão DIEE') union all
select uuid_in('21B89885-9688-EE11-97DC-00155DB4374A'),'Gestão DIEI' where not exists (select * from grupo_gestao where nome = 'Gestão DIEI') union all
select uuid_in('22B89885-9688-EE11-97DC-00155DB4374A'),'Gestão NAAPA SME' where not exists (select * from grupo_gestao where nome = 'Gestão NAAPA SME') union all
select uuid_in('23B89885-9688-EE11-97DC-00155DB4374A'),'Gestão DIEJA' where not exists (select * from grupo_gestao where nome = 'Gestão DIEJA') union all
select uuid_in('24B89885-9688-EE11-97DC-00155DB4374A'),'Gestão DA' where not exists (select * from grupo_gestao where nome = 'Gestão DA') union all
select uuid_in('25B89885-9688-EE11-97DC-00155DB4374A'),'Gestão Multimeios' where not exists (select * from grupo_gestao where nome = 'Gestão Multimeios') union all
select uuid_in('26B89885-9688-EE11-97DC-00155DB4374A'),'Gestão DC' where not exists (select * from grupo_gestao where nome = 'Gestão DC') union all
select uuid_in('27B89885-9688-EE11-97DC-00155DB4374A'),'Gestão NAC' where not exists (select * from grupo_gestao where nome = 'Gestão NAC') union all
select uuid_in('28B89885-9688-EE11-97DC-00155DB4374A'),'Gestão COCEU' where not exists (select * from grupo_gestao where nome = 'Gestão COCEU') union all
select uuid_in('29B89885-9688-EE11-97DC-00155DB4374A'),'Gestão CODAE' where not exists (select * from grupo_gestao where nome = 'Gestão CODAE') union all    
select uuid_in('2AB89885-9688-EE11-97DC-00155DB4374A'),'Gestão DIPED' where not exists (select * from grupo_gestao where nome = 'Gestão DIPED') union all    
select uuid_in('2BB89885-9688-EE11-97DC-00155DB4374A'),'Gestão DICEU' where not exists (select * from grupo_gestao where nome = 'Gestão DICEU')