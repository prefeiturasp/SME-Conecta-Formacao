update inscricao 
set cargo_ue_codigo = '0'||cargo_ue_codigo  
where proposta_turma_id in(select id from proposta_turma pt where pt.proposta_id =32) and not excluido 
and length(cargo_ue_codigo)  = 5;


update inscricao 
set cargo_ue_codigo = '00'||cargo_ue_codigo 
where proposta_turma_id in(select id from proposta_turma pt where pt.proposta_id =32) and not excluido 
and length(cargo_ue_codigo)  = 4;

update inscricao 
set cargo_ue_codigo = '000'||cargo_ue_codigo
where proposta_turma_id in(select id from proposta_turma pt where pt.proposta_id =32) and not excluido 
and length(cargo_ue_codigo)  = 3;


update inscricao 
set cargo_ue_codigo = '0000'||cargo_ue_codigo 
where proposta_turma_id in(select id 
from proposta_turma pt where pt.proposta_id =32) and not excluido 
and length(cargo_ue_codigo)  = 2;