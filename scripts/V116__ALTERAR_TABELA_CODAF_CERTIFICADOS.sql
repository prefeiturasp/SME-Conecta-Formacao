ALTER TABLE CODAF_CERTIFICADOS
ADD COLUMN IF NOT EXISTS codaf_suplementar_inscricao_id int8,
ADD COLUMN IF NOT EXISTS codaf_suplementar_id int8,
ADD CONSTRAINT fk_codaf_certificados_codaf_suplementar_inscricao
FOREIGN KEY (codaf_suplementar_inscricao_id) REFERENCES CODAF_SUPLEMENTAR_INSCRICAO(id),
ADD CONSTRAINT fk_codaf_certificados_codaf_suplementar
FOREIGN KEY (codaf_suplementar_id) REFERENCES CODAF_SUPLEMENTAR(id),
ALTER COLUMN codaf_lista_presenca_id DROP NOT NULL;