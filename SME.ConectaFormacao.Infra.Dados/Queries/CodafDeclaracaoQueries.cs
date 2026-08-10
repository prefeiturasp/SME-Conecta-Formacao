namespace SME.ConectaFormacao.Infra.Dados.Queries
{
    public static class CodafDeclaracaoQueries
    {
        public const string ObterDadosParaEmissao = """
        SELECT 
               CILP.ID AS idReferencia,
               CILP.INSCRICAO_ID AS inscricaoId,
               PT.ID AS propostaTurmaId,
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
               P.HORAS_TOTAIS AS horasTotais,
               P.CARGA_HORARIA_TOTAL_OUTRA AS cargaHorariaTotalOutra,
               U.EMAIL AS emailUsuario,
               P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
               CD.CODIGO_DECLARACAO AS numeroCodigoDeclaracao,
               CASE WHEN P.TIPO_EMISSOR = 2 THEN C_EMISSOR.NOME ELSE D_EMISSOR.NOME END AS emissor,
               CASE WHEN P.TIPO_EMISSOR = 2 THEN C_EMISSOR.SIGLA ELSE NULL END AS emissorSigla,
               P.TIPO_EMISSOR AS tipoEmissor

        FROM   PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO AS CLP
               INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
               INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
               INNER JOIN PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO AS CILP ON CILP.CODAF_CURSO_NAO_HOM_ID = CLP.ID
               INNER JOIN PUBLIC.INSCRICAO AS I ON CILP.INSCRICAO_ID = I.ID 
               INNER JOIN PUBLIC.USUARIO AS U ON I.USUARIO_ID = U.ID
               LEFT JOIN PUBLIC.CODAF_DECLARACOES AS CD ON CD.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO_ID = CILP.ID
               LEFT JOIN PUBLIC.DRE AS D_EMISSOR 
                      ON D_EMISSOR.ID = P.ID_EMISSOR 
                     AND P.TIPO_EMISSOR = 1
                     AND NOT D_EMISSOR.EXCLUIDO
               LEFT JOIN PUBLIC.COORDENADORIA AS C_EMISSOR 
                      ON C_EMISSOR.ID = P.ID_EMISSOR 
                     AND P.TIPO_EMISSOR = 2
                     AND NOT C_EMISSOR.EXCLUIDO
        WHERE  NOT CLP.EXCLUIDO 
          AND  CILP.PARTICIPOU 
          AND  NOT CILP.EXCLUIDO
          AND  CLP.ID = @codafNaoHomologadoId
          AND  P.CURSO_COM_CERTIFICADO = false
        UNION ALL
        SELECT 
               PRT.ID AS idReferencia,
               PRT.ID AS idReferencia,
               PT.ID AS propostaTurmaId,
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
               P.HORAS_TOTAIS horasTotais,
               P.CARGA_HORARIA_TOTAL_OUTRA cargaHorariaTotalOutra,
               U.EMAIL AS emailUsuario,
               P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
               CD.CODIGO_DECLARACAO AS numeroCodigoDeclaracao,
               CASE WHEN P.TIPO_EMISSOR = 2 THEN C_EMISSOR.NOME ELSE D_EMISSOR.NOME END AS emissor,
               CASE WHEN P.TIPO_EMISSOR = 2 THEN C_EMISSOR.SIGLA ELSE NULL END AS emissorSigla,
               P.TIPO_EMISSOR AS tipoEmissor

        FROM   PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO AS CLP
               INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
               INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
               INNER JOIN PUBLIC.PROPOSTA_REGENTE_TURMA AS PRT ON PRT.TURMA_ID = PT.ID
               INNER JOIN PUBLIC.PROPOSTA_REGENTE AS PR  ON PRT.PROPOSTA_REGENTE_ID = PR.ID
               LEFT JOIN PUBLIC.USUARIO AS U ON U.CPF = PR.REGISTRO_FUNCIONAL OR U.LOGIN = PR.REGISTRO_FUNCIONAL
               LEFT JOIN PUBLIC.CODAF_DECLARACOES AS CD ON CD.PROPOSTA_REGENTE_TURMA_ID = PRT.ID
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
          AND  CLP.ID = @codafNaoHomologadoId
          AND  P.CURSO_COM_CERTIFICADO = false
        """;

        public const string AtualizarStatusProcessamento = """
                UPDATE PUBLIC.CODAF_DECLARACOES
                SET STATUS_PROCESSAMENTO = @statusProcessamento,
                    CHAVE_OBJETO_ARMAZENAMENTO = @chaveObjetoArmazenamento,
                    ERRO_PROCESSAMENTO = @erroProcessamento,
                    ALTERADO_EM = NOW(),
                    ALTERADO_POR = 'WORKER'
                WHERE ID = @id
                """;

        public const string ObterParaProcessamento = """
        WITH batch_para_processar AS (
            SELECT id
            FROM   PUBLIC.CODAF_DECLARACOES AS CC
            WHERE  NOT CC.EXCLUIDO 
               AND CC.STATUS_PROCESSAMENTO = @statusPendente
            ORDER  BY id ASC
            LIMIT  @tamanhoLote
            FOR    UPDATE SKIP LOCKED
        ),
        declaracoes_atualizadas AS (
            UPDATE PUBLIC.CODAF_DECLARACOES C
            SET
                STATUS_PROCESSAMENTO = @statusProcessando,
                ALTERADO_EM = NOW(),
                ALTERADO_POR = 'WORKER'
            FROM batch_para_processar B
            WHERE C.id = B.id
            -- Retornamos tudo que precisamos para fazer o JOIN abaixo
            RETURNING C.ID, 
                      C.CODIGO_DECLARACAO, 
                      C.HTML_CONTENT_SNAPSHOT,
                      C.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO_ID,
                      C.PROPOSTA_REGENTE_TURMA_ID
        )
        -- 1. Cursista (Curso Não Homologado)
        SELECT 
               CA.ID,
               CA.CODIGO_DECLARACAO AS codigoDeclaracao,
               CA.HTML_CONTENT_SNAPSHOT AS htmlContentSnapshot,
               U.NOME AS nomeCompleto,
               (U.LOGIN <> U.CPF) AS temRf,
               1 AS tipoParticipacao, -- Cursista
               P.NOME_FORMACAO AS nomeFormacao,
               U.EMAIL AS emailUsuario       
        FROM   declaracoes_atualizadas CA
               INNER JOIN PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO CCNHI ON CA.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO_ID = CCNHI.ID
               INNER JOIN PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO CCNH ON CCNHI.CODAF_CURSO_NAO_HOM_ID = CCNH.ID
               INNER JOIN PUBLIC.PROPOSTA_TURMA PT ON CCNH.PROPOSTA_TURMA_ID = PT.ID
               INNER JOIN PUBLIC.PROPOSTA P ON PT.PROPOSTA_ID = P.ID
               INNER JOIN PUBLIC.INSCRICAO AS I  ON CCNHI.INSCRICAO_ID = I.ID 
               INNER JOIN PUBLIC.USUARIO AS U  ON I.USUARIO_ID = U.ID
        WHERE  CCNHI.PARTICIPOU
               AND  NOT CCNHI.EXCLUIDO

        UNION ALL

        -- 2. Regente
        SELECT        
               CA.ID,
               CA.CODIGO_DECLARACAO AS codigoDeclaracao,
               CA.HTML_CONTENT_SNAPSHOT AS htmlContentSnapshot,
               PR.NOME_REGENTE AS nomeCompleto,
               TRUE AS temRf, -- Regente sempre tem RF
               2 AS tipoParticipacao, -- Regente
               P.NOME_FORMACAO AS nomeFormacao,
               U.EMAIL AS emailUsuario
        FROM   declaracoes_atualizadas CA
               INNER JOIN PUBLIC.PROPOSTA_REGENTE_TURMA AS PRT ON CA.PROPOSTA_REGENTE_TURMA_ID = PRT.ID
               INNER JOIN PUBLIC.PROPOSTA_REGENTE AS PR  ON PRT.PROPOSTA_REGENTE_ID = PR.ID
               INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON PRT.TURMA_ID = PT.ID
               INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
               LEFT JOIN PUBLIC.USUARIO AS U ON U.CPF = PR.REGISTRO_FUNCIONAL OR U.LOGIN = PR.REGISTRO_FUNCIONAL
        WHERE  NOT PRT.EXCLUIDO 
               AND  NOT PR.EXCLUIDO
        """;

        public const string InserirLoteCopy = """
        COPY public.codaf_declaracoes (
            id,
            codigo_declaracao,
            codaf_curso_nao_homologado_inscricao_id,
            codaf_curso_nao_homologado_id,
            proposta_regente_turma_id,
            tipo_participacao,
            data_emissao,
            html_content_snapshot,
            metadados_json,
            status_processamento,
            tentativas_processamento,
            criado_em,
            criado_por,
            criado_login,
            excluido
        ) FROM STDIN (FORMAT BINARY)
        """;

        public const string AtualizarCodigoDeclaracaoNoHtml = """
            UPDATE PUBLIC.CODAF_DECLARACOES CC
            SET HTML_CONTENT_SNAPSHOT = REPLACE(
                REPLACE(CC.HTML_CONTENT_SNAPSHOT, 'NUM_CODIGO_DECLARACAO', CAST(CC.CODIGO_DECLARACAO AS TEXT)),
                'NUM_HOM_FORMACAO',
                (SELECT CAST(P.NUMERO_HOMOLOGACAO AS TEXT)
                 FROM PUBLIC.PROPOSTA_TURMA PT
                 JOIN PUBLIC.PROPOSTA P ON PT.PROPOSTA_ID = P.ID
                 WHERE (PT.ID = (
                    SELECT PROPOSTA_TURMA_ID 
                    FROM PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO 
                    WHERE ID = CC.CODAF_CURSO_NAO_HOMOLOGADO_ID
                 ) OR PT.ID = (
                    SELECT PRT.TURMA_ID 
                    FROM PUBLIC.PROPOSTA_REGENTE_TURMA PRT 
                    WHERE PRT.ID = CC.PROPOSTA_REGENTE_TURMA_ID
                 ))
                 LIMIT 1)
            )
            WHERE (CC.CODAF_CURSO_NAO_HOMOLOGADO_ID = @codafNaoHomologadoId
                   OR CC.PROPOSTA_REGENTE_TURMA_ID IN (
                       SELECT PRT.ID 
                       FROM PUBLIC.PROPOSTA_REGENTE_TURMA PRT
                       WHERE PRT.TURMA_ID IN (
                           SELECT PROPOSTA_TURMA_ID 
                           FROM PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO 
                           WHERE ID = @codafNaoHomologadoId
                       )
                   ))
              AND NOT CC.EXCLUIDO
            """;

        public const string InativarDeclaracoesAnterioresDeCursistas = """
        UPDATE PUBLIC.CODAF_DECLARACOES
            SET EXCLUIDO = TRUE,
                ALTERADO_EM = NOW(),
                ALTERADO_POR = @usuarioNome,
                ALTERADO_LOGIN = @usuarioLogin
            WHERE ID IN (
                SELECT CC.ID
                FROM PUBLIC.CODAF_DECLARACOES CC
                INNER JOIN PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO CSI ON CC.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO_ID = CSI.ID
                WHERE NOT CC.EXCLUIDO
                  AND CSI.INSCRICAO_ID = ANY(@inscricaoId)
            )
        """;
    }
}
