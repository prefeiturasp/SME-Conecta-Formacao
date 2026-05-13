namespace SME.ConectaFormacao.Infra.Dados.Queries
{
    public static class CodafCertificadoQueries
    {
        public const string ObterDadosParaEmissao = """
                SELECT 
                	   CILP.ID AS idReferencia,
                       PT.ID AS propostaTurmaId,
                       CLP.PAGINA_COMUNICADO_DOM AS paginaDiarioOficial,
                       U.NOME AS nomeCompleto,
                       U.LOGIN AS documento,
                       (U.LOGIN <> U.CPF) AS temRf,
                       1 AS tipoParticipacao, -- Cursista
                       P.NOME_FORMACAO AS nomeFormacao,
                       CASE WHEN P.tipo_formacao = 1 THEN 'curso'
                            ELSE 'evento'
                       END AS tipoFormacao,
                       P.DATA_REALIZACAO_INICIO AS dataRealizacao,
                       CILP.CONCEITO_FINAL AS conceitoFinal,
                       CILP.PERCENTUAL_FREQUENCIA AS percentualFrequencia,
                       P.HORAS_TOTAIS AS horasTotais,
                       P.CARGA_HORARIA_TOTAL_OUTRA AS cargaHorariaTotalOutra,
                       U.EMAIL AS emailUsuario,
                       CLP.NUMERO_COMUNICADO AS numeroComunicado,
                       CLP.DATA_PUBLICACAO AS dataPublicacao,
                       P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                       (
                		  SELECT CC.codigo_certificado
                		  FROM PUBLIC.CODAF_CERTIFICADOS CC
                		  WHERE CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID = CILP.ID
                		  FETCH FIRST 1 ROW ONLY
                		) AS codigoCertificado,
                       CASE 
                           WHEN C.NOME IS NOT NULL THEN C.NOME || ' - ' || C.SIGLA
                           ELSE D.NOME
                       END AS dreCoordenadoria
                FROM   PUBLIC.CODAF_LISTA_PRESENCA AS CLP
                       INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
                       INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
                       INNER JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA AS CILP ON CILP.CODAF_LISTA_PRESENCA_ID = CLP.ID
                       INNER JOIN PUBLIC.INSCRICAO AS I ON CILP.INSCRICAO_ID = I.ID 
                       INNER JOIN PUBLIC.USUARIO AS U ON I.USUARIO_ID = U.ID
                       LEFT JOIN PUBLIC.UE AS UE ON UE.CODIGO_UE = U.CODIGO_EOL_UNIDADE
                       LEFT JOIN PUBLIC.DRE AS D ON D.ID = UE.DRE_ID
                       LEFT JOIN PUBLIC.AREA_PROMOTORA AS AP ON AP.DREID = D.ID AND NOT AP.EXCLUIDO
                       LEFT JOIN PUBLIC.COORDENADORIA AS C ON C.ID = AP.COORDENADORIA_ID AND NOT C.EXCLUIDO
                WHERE  NOT CLP.EXCLUIDO 
                  AND  CILP.APROVADO 
                  AND  NOT CILP.EXCLUIDO
                  AND  CLP.ID = @idCodaf
                  AND  P.CURSO_COM_CERTIFICADO
                UNION ALL
                SELECT 
                       PRT.ID AS idReferencia,
                       PT.ID AS propostaTurmaId,
                       CLP.PAGINA_COMUNICADO_DOM AS paginaDiarioOficial,
                       PR.NOME_REGENTE AS nomeCompleto,
                       PR.REGISTRO_FUNCIONAL AS documento,
                       TRUE AS temRf, -- Regente sempre tem RF
                       2 AS tipoParticipacao, -- Regente
                       P.NOME_FORMACAO AS nomeFormacao,                               
                       CASE WHEN P.tipo_formacao = 1 THEN 'curso'
                       ELSE 'evento'
                       END AS tipoFormacao,
                       P.DATA_REALIZACAO_INICIO AS dataRealizacao,
                       NULL AS conceitoFinal,
                       NULL AS percentualFrequencia,
                       P.HORAS_TOTAIS horasTotais,
                       P.CARGA_HORARIA_TOTAL_OUTRA cargaHorariaTotalOutra,
                       U.EMAIL AS emailUsuario,
                       CLP.NUMERO_COMUNICADO AS numeroComunicado,
                       CLP.DATA_PUBLICACAO AS dataPublicacao,
                       P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                       (
                		  SELECT CC.codigo_certificado
                		  FROM PUBLIC.CODAF_CERTIFICADOS CC
                		  JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA CILP2 
                		       ON CILP2.ID = CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID
                		  WHERE CILP2.CODAF_LISTA_PRESENCA_ID = CLP.ID
                		  FETCH FIRST 1 ROW ONLY
                		) AS codigoCertificado,
                       CASE 
                           WHEN C.NOME IS NOT NULL THEN C.NOME || ' - ' || C.SIGLA
                           ELSE D.NOME
                       END AS dreCoordenadoria
                FROM   PUBLIC.CODAF_LISTA_PRESENCA AS CLP
                       INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
                       INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
                       INNER JOIN PUBLIC.PROPOSTA_REGENTE_TURMA AS PRT ON PRT.TURMA_ID = PT.ID
                       INNER JOIN PUBLIC.PROPOSTA_REGENTE AS PR  ON PRT.PROPOSTA_REGENTE_ID = PR.ID
                       LEFT JOIN PUBLIC.USUARIO AS U ON U.CPF = PR.REGISTRO_FUNCIONAL OR U.LOGIN = PR.REGISTRO_FUNCIONAL
                       LEFT JOIN PUBLIC.UE AS UE ON UE.CODIGO_UE = U.CODIGO_EOL_UNIDADE
                       LEFT JOIN PUBLIC.DRE AS D ON D.ID = UE.DRE_ID
                       LEFT JOIN PUBLIC.AREA_PROMOTORA AS AP ON AP.DREID = D.ID AND NOT AP.EXCLUIDO
                       LEFT JOIN PUBLIC.COORDENADORIA AS C ON C.ID = AP.COORDENADORIA_ID AND NOT C.EXCLUIDO
                WHERE  NOT CLP.EXCLUIDO 
                  AND  NOT PRT.EXCLUIDO 
                  AND  NOT PR.EXCLUIDO
                  AND  CLP.ID = @idCodaf
                  AND  P.CURSO_COM_CERTIFICADO
                """;
        public const string InserirLoteCopy = """
                COPY public.codaf_certificados (
                    codaf_lista_presenca_id,
                    codaf_inscricao_lista_presenca_id,
                    proposta_regente_turma_id,
                    tipo_participacao,
                    data_emissao,
                    html_content_snapshot,
                    metadados_json,
                    criado_em,
                    criado_por,
                    criado_login,
                    excluido
                ) FROM STDIN (FORMAT BINARY)
                """;
        public const string ObterParaProcessamento = """
        WITH batch_para_processar AS (
            SELECT id
            FROM   PUBLIC.CODAF_CERTIFICADOS AS CC
            WHERE  NOT CC.EXCLUIDO 
               AND CC.STATUS_PROCESSAMENTO = @statusPendente
            ORDER  BY id ASC
            LIMIT  @tamanhoLote
            FOR    UPDATE SKIP LOCKED
        ),
        certificados_atualizados AS (
            UPDATE PUBLIC.CODAF_CERTIFICADOS C
            SET
                STATUS_PROCESSAMENTO = @statusProcessando,
                ALTERADO_EM = NOW(),
                ALTERADO_POR = 'WORKER'
            FROM batch_para_processar B
            WHERE C.id = B.id
            -- Retornamos tudo que precisamos para fazer o JOIN abaixo
            RETURNING C.ID, 
                      C.CODIGO_CERTIFICADO, 
                      C.HTML_CONTENT_SNAPSHOT,
                      C.CODAF_INSCRICAO_LISTA_PRESENCA_ID, -- FK necessária para o join
                      C.PROPOSTA_REGENTE_TURMA_ID          -- FK necessária para o join
        )
        SELECT 
           	   CA.ID,
        	   CA.CODIGO_CERTIFICADO AS codigoCertificado,
        	   CA.HTML_CONTENT_SNAPSHOT AS htmlContentSnapshot,
               U.NOME AS nomeCompleto,
               (U.LOGIN <> U.CPF) AS temRf,
               1 AS tipoParticipacao, -- Cursista
               P.NOME_FORMACAO AS nomeFormacao,
               U.EMAIL AS emailUsuario       
        FROM   certificados_atualizados CA
        	   INNER JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA CILP ON CA.CODAF_INSCRICAO_LISTA_PRESENCA_ID = CILP.ID
        	   INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA CLP ON CILP.CODAF_LISTA_PRESENCA_ID = CLP.ID
        	   INNER JOIN PUBLIC.PROPOSTA_TURMA PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
        	   INNER JOIN PUBLIC.PROPOSTA P ON PT.PROPOSTA_ID = P.ID
               INNER JOIN PUBLIC.INSCRICAO AS I  ON CILP.INSCRICAO_ID = I.ID 
               INNER JOIN PUBLIC.USUARIO AS U  ON I.USUARIO_ID = U.ID
        UNION ALL
        SELECT        
           	   CA.ID,
        	   CA.CODIGO_CERTIFICADO AS codigoCertificado,
        	   CA.HTML_CONTENT_SNAPSHOT AS htmlContentSnapshot,
        	   PR.NOME_REGENTE AS nomeCompleto,
               TRUE AS temRf, -- Regente sempre tem RF
               2 AS tipoParticipacao, -- Regente
               P.NOME_FORMACAO AS nomeFormacao,
               U.EMAIL AS emailUsuario
        FROM   certificados_atualizados CA
        	   INNER JOIN PUBLIC.PROPOSTA_REGENTE_TURMA AS PRT ON CA.PROPOSTA_REGENTE_TURMA_ID = PRT.ID
               INNER JOIN PUBLIC.PROPOSTA_REGENTE AS PR  ON PRT.PROPOSTA_REGENTE_ID = PR.ID
               INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON PRT.TURMA_ID = PT.ID
               INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CLP.PROPOSTA_TURMA_ID = PT.ID
               INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
               LEFT JOIN PUBLIC.USUARIO AS U ON U.CPF = PR.REGISTRO_FUNCIONAL OR U.LOGIN = PR.REGISTRO_FUNCIONAL
        """;
        public const string AtualizarStatusProcessamento = """
                UPDATE PUBLIC.CODAF_CERTIFICADOS
                SET STATUS_PROCESSAMENTO = @statusProcessamento,
                    CHAVE_OBJETO_ARMAZENAMENTO = @chaveObjetoArmazenamento,
                    ERRO_PROCESSAMENTO = @erroProcessamento,
                    ALTERADO_EM = NOW(),
                    ALTERADO_POR = 'WORKER'
                WHERE ID = @id
                """;
        public const string RecuperarCertificadosTravados = """
                UPDATE PUBLIC.CODAF_CERTIFICADOS
                SET
                    STATUS_PROCESSAMENTO = CASE
                        WHEN tentativas_processamento < 3 THEN @statusPendente
                        ELSE @statusErro
                    END,
                    TENTATIVAS_PROCESSAMENTO = TENTATIVAS_PROCESSAMENTO + 1,
                    ERRO_PROCESSAMENTO = CASE
                        WHEN tentativas_processamento < 3 THEN NULL
                        ELSE 'Erro ao processar certificado'
                    END,
                    ALTERADO_EM = NOW(),
                    ALTERADO_POR = 'WORKER-RESILIENCIA'
                WHERE STATUS_PROCESSAMENTO = @statusProcessando
                  AND ALTERADO_EM < (NOW() - INTERVAL '30 minutes'); -- Mas faz tempo demais, uai!;
                """;
        public const string ObterMeusCertificadosCteBase = """
            WITH BaseCertificados AS (
                SELECT 
                    CC.ID,
                    CC.CODIGO_CERTIFICADO AS codigoCertificado,
                    (U.LOGIN <> U.CPF) AS temRf,
                    1 AS tipoParticipacao, -- Cursista
                    P.NOME_FORMACAO AS nomeFormacao,
                    P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                    CC.DATA_EMISSAO AS dataEmissao,
                    U.LOGIN
                FROM PUBLIC.CODAF_CERTIFICADOS CC
                INNER JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA CILP ON CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID = CILP.ID
                INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA CLP ON CILP.CODAF_LISTA_PRESENCA_ID = CLP.ID
                INNER JOIN PUBLIC.PROPOSTA_TURMA PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
                INNER JOIN PUBLIC.PROPOSTA P ON PT.PROPOSTA_ID = P.ID
                INNER JOIN PUBLIC.INSCRICAO AS I ON CILP.INSCRICAO_ID = I.ID 
                INNER JOIN PUBLIC.USUARIO AS U ON I.USUARIO_ID = U.ID
                WHERE NOT CC.EXCLUIDO AND CC.STATUS_PROCESSAMENTO = @statusProcessado

                UNION ALL

                SELECT        
                    CC.ID,
                    CC.CODIGO_CERTIFICADO AS codigoCertificado,
                    TRUE AS temRf, -- Regente sempre tem RF
                    2 AS tipoParticipacao, -- Regente
                    P.NOME_FORMACAO AS nomeFormacao,
                    P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                    CC.DATA_EMISSAO AS dataEmissao,
                    U.LOGIN
                FROM PUBLIC.CODAF_CERTIFICADOS CC
                INNER JOIN PUBLIC.PROPOSTA_REGENTE_TURMA AS PRT ON CC.PROPOSTA_REGENTE_TURMA_ID = PRT.ID
                INNER JOIN PUBLIC.PROPOSTA_REGENTE AS PR ON PRT.PROPOSTA_REGENTE_ID = PR.ID
                INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON PRT.TURMA_ID = PT.ID
                INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CLP.PROPOSTA_TURMA_ID = PT.ID
                INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
                INNER JOIN PUBLIC.USUARIO AS U ON U.CPF = PR.REGISTRO_FUNCIONAL OR U.LOGIN = PR.REGISTRO_FUNCIONAL
                WHERE NOT CC.EXCLUIDO AND CC.STATUS_PROCESSAMENTO = @statusProcessado
            )
            """;

        public const string ObterCertificadoDisponivelDoUsuario = """
                SELECT 
                    CC.ID,
                    CC.CODIGO_CERTIFICADO AS codigoCertificado,
                    P.NOME_FORMACAO AS nomeFormacao,
                    coalesce(U_ALUNO.NOME, U_PROF.NOME) AS nomeCompleto,
                    CC.CHAVE_OBJETO_ARMAZENAMENTO AS chaveObjetoArmazenamento
                FROM PUBLIC.CODAF_CERTIFICADOS CC
                LEFT JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA CILP ON CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID = CILP.ID
                LEFT JOIN PUBLIC.CODAF_LISTA_PRESENCA CLP ON CILP.CODAF_LISTA_PRESENCA_ID = CLP.ID
                LEFT JOIN PUBLIC.INSCRICAO I ON CILP.INSCRICAO_ID = I.ID 
                LEFT JOIN PUBLIC.USUARIO U_ALUNO ON I.USUARIO_ID = U_ALUNO.ID
                LEFT JOIN PUBLIC.PROPOSTA_REGENTE_TURMA PRT ON CC.PROPOSTA_REGENTE_TURMA_ID = PRT.ID
                LEFT JOIN PUBLIC.PROPOSTA_REGENTE PR ON PRT.PROPOSTA_REGENTE_ID = PR.ID
                LEFT JOIN PUBLIC.USUARIO U_PROF ON (PR.REGISTRO_FUNCIONAL = U_PROF.CPF OR PR.REGISTRO_FUNCIONAL = U_PROF.LOGIN)
                LEFT JOIN PUBLIC.PROPOSTA_TURMA PT ON PT.ID = COALESCE(CLP.PROPOSTA_TURMA_ID, PRT.TURMA_ID)
                LEFT JOIN PUBLIC.PROPOSTA P ON PT.PROPOSTA_ID = P.ID
                WHERE 
                    CC.ID = @certificadoId
                    AND CC.STATUS_PROCESSAMENTO = @statusProcessado
                    AND NOT CC.EXCLUIDO
                    AND (@login IS NULL OR U_ALUNO.LOGIN = @login OR U_PROF.LOGIN = @login)
                """;

        public const string ObterTodosCertificadosBaseJoins = """
                 FROM   PUBLIC.CODAF_CERTIFICADOS AS CC 
                        INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CLP.id = CC.codaf_lista_presenca_id
                        INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON PT.id = CLP.proposta_turma_id
                        INNER JOIN PUBLIC.PROPOSTA AS P ON P.id = PT.proposta_id
                        INNER JOIN PUBLIC.PROPOSTA_DRE AS PD ON PD.PROPOSTA_ID = P.ID                       
                        LEFT JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA AS CILP ON CILP.ID = CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID
                        LEFT JOIN PUBLIC.INSCRICAO AS INSCR ON INSCR.ID = CILP.INSCRICAO_ID
                        LEFT JOIN PUBLIC.USUARIO AS U_Cursista  ON U_Cursista.ID = INSCR.USUARIO_ID
                        LEFT JOIN PUBLIC.PROPOSTA_REGENTE_TURMA AS PRT ON CC.PROPOSTA_REGENTE_TURMA_ID = PRT.ID
                        LEFT JOIN PUBLIC.PROPOSTA_REGENTE AS PR ON PRT.PROPOSTA_REGENTE_ID = PR.ID
                        LEFT JOIN PUBLIC.USUARIO AS U_Regente ON U_Regente.CPF = PR.REGISTRO_FUNCIONAL OR U_Regente.LOGIN = PR.REGISTRO_FUNCIONAL
                """;

        public const string ObterTodosCertificadosSelect = """
                SELECT DISTINCT
                         CC.ID,
                         CC.CODIGO_CERTIFICADO AS codigoCertificado,
                         coalesce(U_Cursista.NOME, U_Regente.NOME, PR.nome_regente) AS nomeParticipante,
                         CASE
             	            WHEN CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID IS NOT NULL THEN @Cursista
             	            WHEN CC.PROPOSTA_REGENTE_TURMA_ID IS NOT NULL THEN @Regente
             	            ELSE @NaoDefinido
                         END AS tipoCertificado,
                         coalesce(U_Cursista.LOGIN, PR.REGISTRO_FUNCIONAL) AS documento,
                         CC.DATA_EMISSAO AS dataEmissao,
                         P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                         P.ID AS codigoFormacao,
                         P.NOME_FORMACAO AS nomeFormacao                        
             """;
    }
}
