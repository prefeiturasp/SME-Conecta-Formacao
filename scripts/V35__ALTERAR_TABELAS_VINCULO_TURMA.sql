alter table proposta_encontro_turma drop column turma;
alter table proposta_encontro_turma add turma_id int8 not null;

alter table proposta_regente_turma drop column turma;
alter table proposta_regente_turma add turma_id int8 not null;

alter table proposta_tutor_turma drop column turma;
alter table proposta_tutor_turma add turma_id int8 not null;

alter table proposta_encontro_turma add constraint proposta_encontro_turma_prop_turma_id_fk foreign key (turma_id) references proposta_turma (id);
alter table proposta_encontro_turma add constraint proposta_regente_turma_prop_turma_id_fk foreign key (turma_id) references proposta_turma (id);
alter table proposta_encontro_turma add constraint proposta_tutor_turma_prop_turma_id_fk foreign key (turma_id) references proposta_turma (id);