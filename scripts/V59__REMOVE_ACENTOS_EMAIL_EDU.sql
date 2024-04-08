-- REMOVER ACENTOS DO EMAIL EDU
UPDATE usuario
SET email_educacional = unaccent(email_educacional);