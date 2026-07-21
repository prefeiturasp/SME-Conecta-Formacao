namespace SME.ConectaFormacao.Infra.Dados.Queries
{
    public static class CodafQueries
    {
        public const string ObterDeltaInscritosCodaf =
        """
        SELECT
        	   CASE
        		   WHEN CI.ID IS NOT NULL AND I.SITUACAO = @situacaoConfirmada AND NOT I.EXCLUIDO THEN @TipoDeltaSemAlteracao
        		   WHEN CI.ID IS NOT NULL AND(I.SITUACAO = @situacaoCancelada OR I.EXCLUIDO) THEN @TipoDeltaRemovido
                   WHEN CI.ID IS NULL AND I.SITUACAO = @situacaoConfirmada AND NOT I.EXCLUIDO THEN @TipoDeltaNovo
        		   ELSE @TipoDeltaIgnorado
        	   END AS tipoDelta,
               I.ID,
               U.LOGIN,
               U.CPF,
               U.NOME,
               CI.PERCENTUAL_FREQUENCIA AS percentualFrequencia,
               CI.ATIVIDADE_OBRIGATORIO AS atividadeObrigatorio,
               CI.CONCEITO_FINAL AS conceitoFinal,
               CI.APROVADO
        FROM   PUBLIC.INSCRICAO AS I 
               INNER JOIN PUBLIC.USUARIO AS U  ON U.ID = I.USUARIO_ID 
               LEFT JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA AS CI ON CI.INSCRICAO_ID = I.ID AND  NOT CI.EXCLUIDO
        WHERE  I.PROPOSTA_TURMA_ID  = @propostaTurmaId
          AND  I.SITUACAO IN (@situacaoConfirmada, @situacaoCancelada)
        ORDER  BY U.NOME, U.LOGIN, U.CPF
        """;

        public const string SqlObterCriteriosCertificacaoPorIdCodaf = """
        SELECT PCC.ID, 
               PCC.PROPOSTA_ID AS PropostaId, 
               PCC.CRITERIO_CERTIFICACAO_ID AS CriterioCertificacaoId
        FROM PUBLIC.PROPOSTA_CRITERIO_CERTIFICACAO AS PCC
        WHERE NOT PCC.EXCLUIDO 
          AND PCC.PROPOSTA_ID = (SELECT PROPOSTA_ID FROM PUBLIC.CODAF_LISTA_PRESENCA WHERE ID = @id LIMIT 1);
        """;
    }
}
