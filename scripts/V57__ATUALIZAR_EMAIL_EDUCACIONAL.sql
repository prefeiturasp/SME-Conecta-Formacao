--Update usuarios internos
   update usuario 
   set email_educacional = lower(substring(trim(nome) from '^[^ ]+'))||lower(substring(trim(nome) from '[^ ]+$'))||'.'||login||'@edu.sme.prefeitura.sp.gov.br'
   where email_educacional isnull and tipo = 1;
  
-- update usuarios externos
   update usuario 
   set email_educacional = lower(substring(trim(nome) from '^[^ ]+'))||lower(substring(trim(nome) from '[^ ]+$'))||'.'||cpf||'@edu.sme.prefeitura.sp.gov.br'
   where email_educacional isnull and tipo = 2;