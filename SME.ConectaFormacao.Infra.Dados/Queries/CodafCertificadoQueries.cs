namespace SME.ConectaFormacao.Infra.Dados.Queries
{
    public static class CodafCertificadoQueries
    {
        public const string ObterDadosParaEmissao = """
        SELECT 
        	   CILP.ID AS idReferencia,
               CILP.INSCRICAO_ID AS inscricaoId,
               PT.ID AS propostaTurmaId,
               CLP.PAGINA_COMUNICADO_DOM AS paginaDiarioOficial,
               U.NOME AS nomeCompleto,
               U.NOME_SOCIAL AS nomeSocial,
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
               CASE WHEN P.TIPO_EMISSOR = 2 THEN C_EMISSOR.NOME ELSE D_EMISSOR.NOME END AS emissor,
               CASE WHEN P.TIPO_EMISSOR = 2 THEN C_EMISSOR.SIGLA ELSE NULL END AS emissorSigla,
               P.TIPO_EMISSOR AS tipoEmissor

        FROM   PUBLIC.CODAF_LISTA_PRESENCA AS CLP
               INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
               INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
               INNER JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA AS CILP ON CILP.CODAF_LISTA_PRESENCA_ID = CLP.ID
               INNER JOIN PUBLIC.INSCRICAO AS I ON CILP.INSCRICAO_ID = I.ID 
               INNER JOIN PUBLIC.USUARIO AS U ON I.USUARIO_ID = U.ID
               LEFT JOIN PUBLIC.DRE AS D_EMISSOR 
                      ON D_EMISSOR.ID = P.ID_EMISSOR 
                     AND P.TIPO_EMISSOR = 1
                     AND NOT D_EMISSOR.EXCLUIDO
               LEFT JOIN PUBLIC.COORDENADORIA AS C_EMISSOR 
                      ON C_EMISSOR.ID = P.ID_EMISSOR 
                     AND P.TIPO_EMISSOR = 2
                     AND NOT C_EMISSOR.EXCLUIDO
        WHERE  NOT CLP.EXCLUIDO 
          AND  CILP.APROVADO 
          AND  NOT CILP.EXCLUIDO
          AND  CLP.ID = @idCodaf
          AND  P.CURSO_COM_CERTIFICADO
        UNION ALL
        SELECT 
               PRT.ID AS idReferencia,
               0 AS inscricaoId,
               PT.ID AS propostaTurmaId,
               CLP.PAGINA_COMUNICADO_DOM AS paginaDiarioOficial,
               PR.NOME_REGENTE AS nomeCompleto,
               NULL AS nomeSocial,
               coalesce(PR.REGISTRO_FUNCIONAL, PR.CPF) AS documento,
               PR.REGISTRO_FUNCIONAL IS NOT NULL AS temRf,
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
               CASE WHEN P.TIPO_EMISSOR = 2 THEN C_EMISSOR.NOME ELSE D_EMISSOR.NOME END AS emissor,
               CASE WHEN P.TIPO_EMISSOR = 2 THEN C_EMISSOR.SIGLA ELSE NULL END AS emissorSigla,
               P.TIPO_EMISSOR AS tipoEmissor
               
        FROM   PUBLIC.CODAF_LISTA_PRESENCA AS CLP
               INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
               INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
               INNER JOIN PUBLIC.PROPOSTA_REGENTE_TURMA AS PRT ON PRT.TURMA_ID = PT.ID
               INNER JOIN PUBLIC.PROPOSTA_REGENTE AS PR  ON PRT.PROPOSTA_REGENTE_ID = PR.ID
               LEFT JOIN PUBLIC.USUARIO AS U ON U.CPF = PR.REGISTRO_FUNCIONAL OR U.LOGIN = PR.REGISTRO_FUNCIONAL
               LEFT JOIN PUBLIC.DRE AS D_EMISSOR 
                      ON D_EMISSOR.ID = P.ID_EMISSOR 
                     AND P.TIPO_EMISSOR = 1
                     AND NOT D_EMISSOR.EXCLUIDO
               LEFT JOIN PUBLIC.COORDENADORIA AS C_EMISSOR 
                      ON C_EMISSOR.ID = P.ID_EMISSOR 
                     AND P.TIPO_EMISSOR = 2
                     AND NOT C_EMISSOR.EXCLUIDO
        WHERE  NOT CLP.EXCLUIDO 
          AND  NOT PRT.EXCLUIDO 
          AND  NOT PR.EXCLUIDO
          AND  CLP.ID = @idCodaf
          AND  P.CURSO_COM_CERTIFICADO
        """;

        public const string ObterDadosParaEmissaoSuplementar = """
                
        SELECT 
           	  CSI.ID AS idReferencia,
              CSI.INSCRICAO_ID AS inscricaoId,
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
               CSI.CONCEITO_FINAL AS conceitoFinal,
               CSI.PERCENTUAL_FREQUENCIA AS percentualFrequencia,
               P.HORAS_TOTAIS AS horasTotais,
               P.CARGA_HORARIA_TOTAL_OUTRA AS cargaHorariaTotalOutra,
               U.EMAIL AS emailUsuario,
               CLP.NUMERO_COMUNICADO AS numeroComunicado,
               CLP.DATA_PUBLICACAO AS dataPublicacao,
               P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
               CASE WHEN P.TIPO_EMISSOR = 2 THEN C_EMISSOR.NOME ELSE D_EMISSOR.NOME END AS emissor,
               CASE WHEN P.TIPO_EMISSOR = 2 THEN C_EMISSOR.SIGLA ELSE NULL END AS emissorSigla,
               P.TIPO_EMISSOR AS tipoEmissor
        FROM   PUBLIC.CODAF_SUPLEMENTAR AS CS
               INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CS.CODAF_LISTA_PRESENCA_ID  = CLP.ID
               INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
               INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
               INNER JOIN PUBLIC.CODAF_SUPLEMENTAR_INSCRICAO AS CSI ON CSI.CODAF_SUPLEMENTAR_ID = CS.ID
               INNER JOIN PUBLIC.INSCRICAO AS I ON CSI.INSCRICAO_ID = I.ID 
               INNER JOIN PUBLIC.USUARIO AS U ON I.USUARIO_ID = U.ID
               LEFT JOIN PUBLIC.DRE AS D_EMISSOR ON D_EMISSOR.ID = P.ID_EMISSOR 
                     AND P.TIPO_EMISSOR = 1
                     AND NOT D_EMISSOR.EXCLUIDO
               LEFT JOIN PUBLIC.COORDENADORIA AS C_EMISSOR ON C_EMISSOR.ID = P.ID_EMISSOR 
                     AND P.TIPO_EMISSOR = 2
                     AND NOT C_EMISSOR.EXCLUIDO
        WHERE  NOT CLP.EXCLUIDO 
          AND  CSI.APROVADO 
          AND  NOT CSI.EXCLUIDO
          AND  CS.ID = @codafSuplementarId
          AND  P.CURSO_COM_CERTIFICADO
        """;

        public const string InserirLoteCopy = """
        COPY public.codaf_certificados (
            codaf_lista_presenca_id,
            codaf_suplementar_id,
            codaf_inscricao_lista_presenca_id,
            codaf_suplementar_inscricao_id,
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
                      C.CODAF_SUPLEMENTAR_INSCRICAO_ID,    -- FK necessária para o join
                      C.PROPOSTA_REGENTE_TURMA_ID          -- FK necessária para o join
        )
        -- 1. Cursista (Lista de Presença Padrão)
        SELECT 
               CA.ID,
               CA.CODIGO_CERTIFICADO AS codigoCertificado,
               CA.HTML_CONTENT_SNAPSHOT AS htmlContentSnapshot,
               U.NOME AS nomeCompleto,
               U.NOME_SOCIAL AS nomeSocial,
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

        -- 2. Cursista (Lista Suplementar)
        SELECT 
               CA.ID,
               CA.CODIGO_CERTIFICADO AS codigoCertificado,
               CA.HTML_CONTENT_SNAPSHOT AS htmlContentSnapshot,
               U.NOME AS nomeCompleto,
               U.NOME_SOCIAL AS nomeSocial,
               (U.LOGIN <> U.CPF) AS temRf,
               1 AS tipoParticipacao, -- Cursista
               P.NOME_FORMACAO AS nomeFormacao,
               U.EMAIL AS emailUsuario       
        FROM   certificados_atualizados CA
               INNER JOIN PUBLIC.CODAF_SUPLEMENTAR_INSCRICAO CSI ON CA.CODAF_SUPLEMENTAR_INSCRICAO_ID = CSI.ID
               INNER JOIN PUBLIC.CODAF_SUPLEMENTAR CS ON CSI.CODAF_SUPLEMENTAR_ID = CS.ID
               INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA CLP ON CS.CODAF_LISTA_PRESENCA_ID = CLP.ID
               INNER JOIN PUBLIC.PROPOSTA_TURMA PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
               INNER JOIN PUBLIC.PROPOSTA P ON PT.PROPOSTA_ID = P.ID
               INNER JOIN PUBLIC.INSCRICAO AS I  ON CSI.INSCRICAO_ID = I.ID 
               INNER JOIN PUBLIC.USUARIO AS U  ON I.USUARIO_ID = U.ID

        UNION ALL

        -- 3. Regente
        SELECT        
               CA.ID,
               CA.CODIGO_CERTIFICADO AS codigoCertificado,
               CA.HTML_CONTENT_SNAPSHOT AS htmlContentSnapshot,
               PR.NOME_REGENTE AS nomeCompleto,
               CAST(NULL AS VARCHAR) AS nomeSocial,
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
                -- 1. Cursista (Lista Normal)
                SELECT 
                    CC.ID, CC.CODIGO_CERTIFICADO AS codigoCertificado, 
                    (U.LOGIN <> U.CPF) AS temRf,
                    1 AS tipoParticipacao, 
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

                -- 2. Cursista (Lista Suplementar)
                SELECT 
                    CC.ID, 
                    CC.CODIGO_CERTIFICADO AS codigoCertificado, 
                    (U.LOGIN <> U.CPF) AS temRf,
                    1 AS tipoParticipacao, 
                    P.NOME_FORMACAO AS nomeFormacao, 
                    P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                    CC.DATA_EMISSAO AS dataEmissao, 
                    U.LOGIN
                FROM PUBLIC.CODAF_CERTIFICADOS CC
                INNER JOIN PUBLIC.CODAF_SUPLEMENTAR_INSCRICAO CSI ON CC.CODAF_SUPLEMENTAR_INSCRICAO_ID = CSI.ID
                INNER JOIN PUBLIC.CODAF_SUPLEMENTAR CS ON CSI.CODAF_SUPLEMENTAR_ID = CS.ID
                INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA CLP ON CS.CODAF_LISTA_PRESENCA_ID = CLP.ID
                INNER JOIN PUBLIC.PROPOSTA_TURMA PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
                INNER JOIN PUBLIC.PROPOSTA P ON PT.PROPOSTA_ID = P.ID
                INNER JOIN PUBLIC.INSCRICAO AS I ON CSI.INSCRICAO_ID = I.ID 
                INNER JOIN PUBLIC.USUARIO AS U ON I.USUARIO_ID = U.ID
                WHERE NOT CC.EXCLUIDO AND CC.STATUS_PROCESSAMENTO = @statusProcessado

                UNION ALL

                -- 3. Regente
                SELECT        
                    CC.ID, CC.CODIGO_CERTIFICADO AS codigoCertificado, 
                    TRUE AS temRf, 
                    2 AS tipoParticipacao, 
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
            WITH CertificadoBase AS (
                SELECT 
                    CC.ID, 
                    CC.CODIGO_CERTIFICADO AS codigoCertificado, 
                    P.NOME_FORMACAO AS nomeFormacao, 
                    U.NOME AS nomeCompleto, 
                    CC.CHAVE_OBJETO_ARMAZENAMENTO AS chaveObjetoArmazenamento, 
                    U.LOGIN AS loginParticipante
                FROM PUBLIC.CODAF_CERTIFICADOS CC
                INNER JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA CILP ON CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID = CILP.ID
                INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA CLP ON CILP.CODAF_LISTA_PRESENCA_ID = CLP.ID
                INNER JOIN PUBLIC.PROPOSTA_TURMA PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
                INNER JOIN PUBLIC.PROPOSTA P ON PT.PROPOSTA_ID = P.ID
                INNER JOIN PUBLIC.INSCRICAO I ON CILP.INSCRICAO_ID = I.ID 
                INNER JOIN PUBLIC.USUARIO U ON I.USUARIO_ID = U.ID
                WHERE CC.ID = @certificadoId AND CC.STATUS_PROCESSAMENTO = @statusProcessado AND NOT CC.EXCLUIDO

                UNION ALL

                SELECT 
                    CC.ID, 
                    CC.CODIGO_CERTIFICADO AS codigoCertificado, 
                    P.NOME_FORMACAO AS nomeFormacao, 
                    U.NOME AS nomeCompleto, 
                    CC.CHAVE_OBJETO_ARMAZENAMENTO AS chaveObjetoArmazenamento, 
                    U.LOGIN AS loginParticipante
                FROM PUBLIC.CODAF_CERTIFICADOS CC
                INNER JOIN PUBLIC.CODAF_SUPLEMENTAR_INSCRICAO CSI ON CC.CODAF_SUPLEMENTAR_INSCRICAO_ID = CSI.ID
                INNER JOIN PUBLIC.CODAF_SUPLEMENTAR CS ON CSI.CODAF_SUPLEMENTAR_ID = CS.ID
                INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA CLP ON CS.CODAF_LISTA_PRESENCA_ID = CLP.ID
                INNER JOIN PUBLIC.PROPOSTA_TURMA PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
                INNER JOIN PUBLIC.PROPOSTA P ON PT.PROPOSTA_ID = P.ID
                INNER JOIN PUBLIC.INSCRICAO I ON CSI.INSCRICAO_ID = I.ID 
                INNER JOIN PUBLIC.USUARIO U ON I.USUARIO_ID = U.ID
                WHERE CC.ID = @certificadoId AND CC.STATUS_PROCESSAMENTO = @statusProcessado AND NOT CC.EXCLUIDO

                UNION ALL

                SELECT 
                    CC.ID, 
                    CC.CODIGO_CERTIFICADO AS codigoCertificado, 
                    P.NOME_FORMACAO AS nomeFormacao, 
                    PR.NOME_REGENTE AS nomeCompleto, 
                    CC.CHAVE_OBJETO_ARMAZENAMENTO AS chaveObjetoArmazenamento, 
                    coalesce(PR.REGISTRO_FUNCIONAL, PR.CPF) AS loginParticipante
                FROM PUBLIC.CODAF_CERTIFICADOS CC
                INNER JOIN PUBLIC.PROPOSTA_REGENTE_TURMA PRT ON CC.PROPOSTA_REGENTE_TURMA_ID = PRT.ID
                INNER JOIN PUBLIC.PROPOSTA_REGENTE PR ON PRT.PROPOSTA_REGENTE_ID = PR.ID
                INNER JOIN PUBLIC.PROPOSTA_TURMA PT ON PRT.TURMA_ID = PT.ID
                INNER JOIN PUBLIC.PROPOSTA P ON PT.PROPOSTA_ID = P.ID
                WHERE CC.ID = @certificadoId AND CC.STATUS_PROCESSAMENTO = @statusProcessado AND NOT CC.EXCLUIDO
            )
            SELECT ID, codigoCertificado, nomeFormacao, nomeCompleto, chaveObjetoArmazenamento
            FROM CertificadoBase
            WHERE (@login IS NULL OR loginParticipante = @login)
            """;

        public const string ObterTodosCertificadosCteBase = """
            WITH BaseCertificados AS (
                -- 1. Cursista (Lista Normal)
                SELECT 
                    CC.ID AS id, 
                    CC.CODIGO_CERTIFICADO AS codigoCertificado, 
                    COALESCE(NULLIF(TRIM(U.NOME_SOCIAL), ''), U.NOME) AS nomeParticipante,
                    @Cursista AS tipoCertificado, 
                    U.LOGIN AS documento, 
                    CC.DATA_EMISSAO AS dataEmissao,
                    P.NUMERO_HOMOLOGACAO AS numeroHomologacao, 
                    P.ID AS codigoFormacao,
                    P.NOME_FORMACAO AS nomeFormacao, 
                    PD.DRE_ID AS dreId, 
                    CLP.PROPOSTA_TURMA_ID AS propostaTurmaId
                FROM PUBLIC.CODAF_CERTIFICADOS CC
                INNER JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA CILP ON CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID = CILP.ID
                INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA CLP ON CILP.CODAF_LISTA_PRESENCA_ID = CLP.ID
                INNER JOIN PUBLIC.PROPOSTA_TURMA PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
                INNER JOIN PUBLIC.PROPOSTA P ON PT.PROPOSTA_ID = P.ID
                INNER JOIN PUBLIC.PROPOSTA_DRE PD ON PD.PROPOSTA_ID = P.ID
                INNER JOIN PUBLIC.INSCRICAO INSCR ON CILP.INSCRICAO_ID = INSCR.ID
                INNER JOIN PUBLIC.USUARIO U ON INSCR.USUARIO_ID = U.ID
                WHERE NOT CC.EXCLUIDO AND CC.STATUS_PROCESSAMENTO = @processadoComSucesso

                UNION ALL

                -- 2. Cursista (Lista Suplementar)
                SELECT 
                    CC.ID AS id, 
                    CC.CODIGO_CERTIFICADO AS codigoCertificado, 
                    U.NOME AS nomeParticipante,
                    @Cursista AS tipoCertificado, 
                    U.LOGIN AS documento, 
                    CC.DATA_EMISSAO AS dataEmissao,
                    P.NUMERO_HOMOLOGACAO AS numeroHomologacao, 
                    P.ID AS codigoFormacao,
                    P.NOME_FORMACAO AS nomeFormacao, 
                    PD.DRE_ID AS dreId, 
                    CLP.PROPOSTA_TURMA_ID AS propostaTurmaId
                FROM PUBLIC.CODAF_CERTIFICADOS CC
                INNER JOIN PUBLIC.CODAF_SUPLEMENTAR_INSCRICAO CSI ON CC.CODAF_SUPLEMENTAR_INSCRICAO_ID = CSI.ID
                INNER JOIN PUBLIC.CODAF_SUPLEMENTAR CS ON CSI.CODAF_SUPLEMENTAR_ID = CS.ID
                INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA CLP ON CS.CODAF_LISTA_PRESENCA_ID = CLP.ID
                INNER JOIN PUBLIC.PROPOSTA_TURMA PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
                INNER JOIN PUBLIC.PROPOSTA P ON PT.PROPOSTA_ID = P.ID
                INNER JOIN PUBLIC.PROPOSTA_DRE PD ON PD.PROPOSTA_ID = P.ID
                INNER JOIN PUBLIC.INSCRICAO INSCR ON CSI.INSCRICAO_ID = INSCR.ID
                INNER JOIN PUBLIC.USUARIO U ON INSCR.USUARIO_ID = U.ID
                WHERE NOT CC.EXCLUIDO AND CC.STATUS_PROCESSAMENTO = @processadoComSucesso

                UNION ALL

                -- 3. Regente
                SELECT 
                    CC.ID AS id, 
                    CC.CODIGO_CERTIFICADO AS codigoCertificado, 
                    PR.NOME_REGENTE AS nomeParticipante,
                    @Regente AS tipoCertificado, 
                    coalesce(PR.REGISTRO_FUNCIONAL, PR.CPF) AS documento, 
                    CC.DATA_EMISSAO AS dataEmissao,
                    P.NUMERO_HOMOLOGACAO AS numeroHomologacao, 
                    P.ID AS codigoFormacao,
                    P.NOME_FORMACAO AS nomeFormacao, 
                    PD.DRE_ID AS dreId, 
                    CLP.PROPOSTA_TURMA_ID AS propostaTurmaId
                FROM PUBLIC.CODAF_CERTIFICADOS CC
                INNER JOIN PUBLIC.PROPOSTA_REGENTE_TURMA PRT ON CC.PROPOSTA_REGENTE_TURMA_ID = PRT.ID
                INNER JOIN PUBLIC.PROPOSTA_REGENTE PR ON PRT.PROPOSTA_REGENTE_ID = PR.ID
                INNER JOIN PUBLIC.PROPOSTA_TURMA PT ON PRT.TURMA_ID = PT.ID
                INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA CLP ON CLP.PROPOSTA_TURMA_ID = PT.ID
                INNER JOIN PUBLIC.PROPOSTA P ON PT.PROPOSTA_ID = P.ID
                INNER JOIN PUBLIC.PROPOSTA_DRE PD ON PD.PROPOSTA_ID = P.ID
                WHERE NOT CC.EXCLUIDO AND CC.STATUS_PROCESSAMENTO = @processadoComSucesso
            )
            """;

        public const string AtualizarCodigoCertificadoNoHtml = """
            UPDATE PUBLIC.CODAF_CERTIFICADOS
            SET HTML_CONTENT_SNAPSHOT = REPLACE(HTML_CONTENT_SNAPSHOT, 'NUM_CODIGO_CERTIFICADO', CAST(CODIGO_CERTIFICADO AS TEXT))
            WHERE (CODAF_LISTA_PRESENCA_ID = @codafId AND @tipoCodaf = 1) OR (CODAF_SUPLEMENTAR_ID = @codafId AND @tipoCodaf = 2)
              AND NOT EXCLUIDO
            """;

        public const string InativarCertificadosAnterioresDeCursistas = """
        UPDATE PUBLIC.CODAF_CERTIFICADOS
            SET EXCLUIDO = TRUE,
                ALTERADO_EM = NOW(),
                ALTERADO_POR = @usuarioNome,
                ALTERADO_LOGIN = @usuarioLogin
            WHERE ID IN (
                SELECT CC.ID
                FROM PUBLIC.CODAF_CERTIFICADOS CC
                LEFT JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA CILP ON CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID = CILP.ID
                LEFT JOIN PUBLIC.CODAF_SUPLEMENTAR_INSCRICAO CSI ON CC.CODAF_SUPLEMENTAR_INSCRICAO_ID = CSI.ID
                WHERE NOT CC.EXCLUIDO
        		  AND (CILP.INSCRICAO_ID = ANY(@inscricaoId) OR CSI.INSCRICAO_ID = ANY(@inscricaoId))
            )
        """;
    }
}
