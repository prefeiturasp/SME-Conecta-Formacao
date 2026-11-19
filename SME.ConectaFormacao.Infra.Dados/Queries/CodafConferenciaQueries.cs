using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Queries
{
    [ExcludeFromCodeCoverage]
    public static class CodafConferenciaQueries
    {
        public const string sqlObterListagemCodaf = """
            SELECT CC.ID,
                   P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                   P.NOME_FORMACAO AS nomeFormacao,
                   P.ID AS codigoFormacao,
                   PT.NOME AS nomeTurma,
                   AP.NOME AS nomeAreaPromotora,
                   CC.STATUS,
                   CASE
                       WHEN P.CURSO_COM_CERTIFICADO = FALSE THEN 0
                       ELSE 1
                   END AS statusCertificacaoTurma,
                   CC.CODIGO_CURSO_EOL AS codigoCursoEol,
                   CC.CODIGO_NIVEL AS codigoNivel,
                   CC.CRIADO_EM AS criadoEm,
                   EXISTS (SELECT 1 
                           FROM   CODAF_CONFERENCIA_INSCRICAO AS CCI 
                           WHERE  NOT CCI.EXCLUIDO 
                             AND  CCI.CODAF_CONFERENCIA_ID = CC.ID 
                             AND  CCI.PARTICIPOU = TRUE) AS PossuiAprovacoes
            """;

        public const string sqlObterCodafPorIdComPropostaEPropostaTurma = """
            SELECT CC.ID as Id,
                   CC.PROPOSTA_ID AS propostaId,
                   CC.PROPOSTA_TURMA_ID AS propostaTurmaId,
                   CC.NUMERO_COMUNICADO AS numeroComunicado,
                   CC.DATA_COMUNICADO AS dataComunicado,
                   CC.PAGINA_COMUNICADO_DOM AS paginaComunicadoDom,
                   CC.DATA_PUBLICACAO_DOM AS dataPublicacaoDom,
                   CC.CODIGO_CURSO_EOL AS codigoCursoEol,
                   CC.CODIGO_NIVEL AS codigoNivel,
                   CC.OBSERVACAO,
                   CC.STATUS,
                   CC.DATA_FINALIZACAO AS dataFinalizacao,
                   CC.ALTERADO_EM AS alteradoEm,
                   CC.ALTERADO_POR AS alteradoPor,
                   CC.ALTERADO_LOGIN AS alteradoLogin,
                   CC.CRIADO_EM AS criadoEm,
                   CC.CRIADO_POR AS criadoPor,
                   CC.CRIADO_LOGIN AS criadoLogin,
           
                   P.ID, 
                   P.NOME_FORMACAO AS nomeFormacao,
                   P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
           
                   PT.ID, 
                   PT.NOME
            FROM PUBLIC.CODAF_CONFERENCIA AS CC
            INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CC.PROPOSTA_TURMA_ID = PT.ID
            INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
            WHERE NOT CC.EXCLUIDO AND NOT PT.EXCLUIDO AND NOT P.EXCLUIDO 
              AND CC.ID = @id;
            """;

        public const string sqlObterAnexosPorIdCodaf = """
            SELECT CA.ID, 
                   CA.CODAF_CONFERENCIA_ID AS CodafConferenciaId,
                   CA.ARQUIVO_CODIGO AS ArquivoCodigo,
                   CA.NOME_ARQUIVO AS NomeArquivo,
                   CA.EXTENSAO AS Extensao,
                   CA.TIPO_ANEXO_ID AS TipoAnexoId,
                   CA.CRIADO_EM AS CriadoEm,
                   CA.CRIADO_POR AS CriadoPor
            FROM PUBLIC.CODAF_CONFERENCIA_ANEXO AS CA 
            WHERE NOT CA.EXCLUIDO AND CA.CODAF_CONFERENCIA_ID = @id;
            """;

        public const string sqlObterInscricoesDaListaPorIdCodaf = """
            SELECT CCI.ID, 
                   CCI.CODAF_CONFERENCIA_ID AS CodafConferenciaId,
                   CCI.INSCRICAO_ID AS InscricaoId,
                   CCI.PARTICIPOU AS Participou,
                   CCI.CRIADO_EM AS CriadoEm,
                   CCI.CRIADO_POR AS CriadoPor,
                   CCI.CRIADO_LOGIN AS CriadoLogin
            FROM PUBLIC.CODAF_CONFERENCIA_INSCRICAO AS CCI
            INNER JOIN PUBLIC.INSCRICAO AS I ON CCI.INSCRICAO_ID = I.ID
            INNER JOIN PUBLIC.USUARIO AS U ON I.USUARIO_ID = U.ID
            WHERE NOT CCI.EXCLUIDO AND CCI.CODAF_CONFERENCIA_ID = @id;
            """;
    }
}
