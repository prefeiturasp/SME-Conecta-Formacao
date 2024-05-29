------------------------------------------------------------------------------------------------------------------------------------------------------
--> Levantamento dos casos
SELECT 
    id, 
    nome,
    email, 
    login, 
    email_educacional,
    LOWER(
        TRANSLATE(
            UNACCENT(
                CASE 
                    WHEN POSITION(' ' IN nome) > 0 THEN
                        SPLIT_PART(TRIM(nome), ' ', 1) ||                        
                        REVERSE(SPLIT_PART(REVERSE(TRIM(nome)), ' ', 1))
                    ELSE
                        nome
                END
            ),
            'áàâãäéèêëíìîïóòôõöúùûüç',
            'aaaaaeeeeiiiiooooouuuuc'
        ) ||
        '.' ||
        login ||
        '@edu.sme.prefeitura.sp.gov.br'
    ) AS email_educacional_corrigido
into temp_usuario_edu
FROM usuario
WHERE LENGTH(nome) > LENGTH(RTRIM(nome))
ORDER BY 1 DESC;

------------------------------------------------------------------------------------------------------------------------------------------------------
--> Atualização de nome e e-mail conforme padrão
UPDATE usuario u
SET nome = rtrim(ltrim(tue.nome)),
       email_educacional = tue.email_educacional_corrigido
FROM temp_usuario_edu tue
WHERE u.id = tue.id;

------------------------------------------------------------------------------------------------------------------------------------------------------
--> Validação

--> Usuarios com espaços nos nomes
select * from usuario WHERE LENGTH(nome) > LENGTH(RTRIM(nome))

--> Usuarios alvos
select * from temp_usuario_edu

--> Usuarios vs Alvos
select t1.nome, t2.* from usuario t1 join temp_usuario_edu t2 on t1.id = t2.id

/*
drop table temp_usuario_edu
 */














