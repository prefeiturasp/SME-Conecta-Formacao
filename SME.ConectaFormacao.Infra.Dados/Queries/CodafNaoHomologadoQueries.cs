namespace SME.ConectaFormacao.Infra.Dados.Queries
{
    public static class CodafNaoHomologadoQueries
    {
        public const string sqlObterListagemCodaf = """
            SELECT CCNH.ID,
                   P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                   p.NOME_FORMACAO AS nomeFormacao,
                   p.ID AS codigoFormacao,
                   pt.NOME AS nomeTurma,
                   ap.NOME AS nomeAreaPromotora,
                   CCNH.STATUS,
                   CASE       	
                   	-- 1: Não emitidas
                   	WHEN CCNH.STATUS = 1 THEN 1
            
            	    -- 0: Sem declarações
                   	WHEN NOT EXISTS (SELECT 1 
                   	                 FROM   CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO AS CCNHI 
                   	                 WHERE  NOT CCNHI.EXCLUIDO 
                   	                   AND  CCNHI.CODAF_CURSO_NAO_HOM_ID = CCNH.ID 
                   	                   AND  CCNHI.PARTICIPOU
                   	) THEN 0
            
                   	-- 3: Declarações em processamento
                   	WHEN EXISTS (SELECT 1 
               	                 FROM   CODAF_DECLARACOES CD 
               	                 WHERE  NOT CD.EXCLUIDO
               	                   AND  CD.CODAF_CURSO_NAO_HOMOLOGADO_ID = CCNH.ID
               	                   AND  CD.STATUS_PROCESSAMENTO IN (@statusPendente, @statusEmProcessamento)
                   	) THEN 3
            
                   	-- 4: Emitido
                   	WHEN EXISTS (SELECT 1 
               	                 FROM   CODAF_DECLARACOES CD 
               	                 WHERE  NOT CD.EXCLUIDO
               	                   AND  CD.CODAF_CURSO_NAO_HOMOLOGADO_ID = CCNH.ID
               	                   AND  CD.STATUS_PROCESSAMENTO IN (@statusProcessadoComSucesso, @statusProcessadoComErro)
                   	) THEN 4
            
                   	-- 2: Disponível para Emissão
                   	ELSE 2
                   END AS statusDeclaracaoTurma
            """;

        public const string sqlObterCodafPorIdComPropostaEPropostaTurma = """
            SELECT CCNH.ID as Id,
                   CCNH.PROPOSTA_ID AS propostaId,
                   CCNH.PROPOSTA_TURMA_ID AS propostaTurmaId,
                   CCNH.OBSERVACAO,
                   CCNH.STATUS,
                   CCNH.ALTERADO_EM AS alteradoEm,
                   CCNH.ALTERADO_POR AS alteradoPor,
                   CCNH.ALTERADO_LOGIN AS alteradoLogin,
                   CCNH.CRIADO_EM AS criadoEm,
                   CCNH.CRIADO_POR AS criadoPor,
                   CCNH.CRIADO_LOGIN AS criadoLogin,
           
                   P.ID, 
                   P.NOME_FORMACAO AS nomeFormacao,
                   P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
           
                   PT.ID, 
                   PT.NOME
            FROM PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO AS CCNH
            INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CCNH.PROPOSTA_TURMA_ID = PT.ID
            INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
            WHERE NOT CCNH.EXCLUIDO AND NOT PT.EXCLUIDO AND NOT P.EXCLUIDO 
              AND CCNH.ID = @id;
            """;

        public const string sqlObterAnexosPorIdCodaf = """
            SELECT CA.ID, 
                   CA.CODAF_CURSO_NAO_HOM_ID AS CodafCursoNaoHomologadoId,
                   CA.ARQUIVO_CODIGO AS ArquivoCodigo,
                   CA.NOME_ARQUIVO AS NomeArquivo,
                   CA.EXTENSAO AS Extensao,
                   CA.CRIADO_EM AS CriadoEm,
                   CA.CRIADO_POR AS CriadoPor
            FROM PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO_ANEXO AS CA 
            WHERE NOT CA.EXCLUIDO AND CA.CODAF_CURSO_NAO_HOM_ID = @id;
            """;

        public const string sqlObterInscricoesDaListaPorIdCodaf = """
            SELECT CILP.ID, 
                   CILP.CODAF_CURSO_NAO_HOM_ID AS CodafCursoNaoHomologadoId,
                   CILP.INSCRICAO_ID AS InscricaoId,
                   CILP.PARTICIPOU AS Participou,
                   CILP.CRIADO_EM AS CriadoEm,
                   CILP.CRIADO_POR AS CriadoPor,
                   CILP.CRIADO_LOGIN AS CriadoLogin,
                   U.NOME AS Nome,
                   U.LOGIN AS Login,
                   U.CPF AS Cpf
            FROM PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO AS CILP
            INNER JOIN PUBLIC.INSCRICAO AS I ON CILP.INSCRICAO_ID = I.ID
            INNER JOIN PUBLIC.USUARIO AS U ON I.USUARIO_ID = U.ID
            WHERE NOT CILP.EXCLUIDO AND CILP.CODAF_CURSO_NAO_HOM_ID = @id;
            """;

        public const string sqlObterDeclaracoesPorIdCodaf = """
            SELECT CD.ID,
                   CD.CODAF_CURSO_NAO_HOMOLOGADO_ID AS CodafCursoNaoHomologadoId
            FROM PUBLIC.CODAF_DECLARACOES AS CD
            WHERE NOT CD.EXCLUIDO AND CD.CODAF_CURSO_NAO_HOMOLOGADO_ID = @id;
            """;
    }
}