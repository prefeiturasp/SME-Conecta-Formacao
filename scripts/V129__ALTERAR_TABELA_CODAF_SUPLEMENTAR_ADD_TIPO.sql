ALTER TABLE PUBLIC.CODAF_SUPLEMENTAR
    ADD COLUMN IF NOT EXISTS tipo int NOT NULL DEFAULT 1,
    ADD COLUMN IF NOT EXISTS codaf_curso_nao_homologado_id int8 NULL,
    ALTER COLUMN codaf_lista_presenca_id DROP NOT NULL;

ALTER TABLE PUBLIC.CODAF_SUPLEMENTAR
    DROP CONSTRAINT IF EXISTS codaf_suplementar_ccnh_id_fk,
    ADD CONSTRAINT codaf_suplementar_ccnh_id_fk
        FOREIGN KEY (codaf_curso_nao_homologado_id) REFERENCES PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO(id);

ALTER TABLE PUBLIC.CODAF_SUPLEMENTAR
    DROP CONSTRAINT IF EXISTS codaf_suplementar_vinculo_tipo_ck,
    ADD CONSTRAINT codaf_suplementar_vinculo_tipo_ck CHECK (
        (tipo = 1 AND codaf_lista_presenca_id IS NOT NULL AND codaf_curso_nao_homologado_id IS NULL)
     OR (tipo = 2 AND codaf_curso_nao_homologado_id IS NOT NULL AND codaf_lista_presenca_id IS NULL)
    );