namespace SME.ConectaFormacao.Infra.Dados.Queries
{
    public static class RelatoriosInscritosQueries
    {
        public const string ObterInscritosPorFormacao =
            """
            WITH inscritos_rankeados AS (
                SELECT P.ID AS codigoFormacao,
                       P.NUMERO_HOMOLOGACAO AS codigoHomologacao,
                       P.NOME_FORMACAO AS nomeFormacao, 
                       AP.NOME AS areaPromotora, 
                       D.NOME AS dre, 
                       UE.NOME_ESCOLA AS ue, 
                       P.DATA_REALIZACAO_INICIO as dataRealizacaoInicio,
                       P.DATA_REALIZACAO_FIM as dataRealizacaoFim, 
                       P.SITUACAO AS situacaoFormacao, 
                       P.FORMATO AS modalidadeFormativa, 
                       CASE 
                           WHEN U.TIPO = 2 THEN 'Estudante de Estágio'
                           WHEN U.TIPO = 3 THEN 'Funcionário de Unidades Parceiras'
                           ELSE CF_CARGO.NOME
                       END AS publicoAlvo, 
                       CF_FUNC_PROP.NOME AS funcaoEspecifica, 
                       PM.MODALIDADE AS etapaModalidade, 
                       AT.DESCRICAO AS anoEtapa, 
                       CC.NOME AS componenteCurricular, 
                       PT.NOME AS turma, 
                       U.LOGIN AS rfCpf, 
                       U.NOME AS nomeCursista, 
                       UA.POSSUI_DEFICIENCIA AS pcd, 
                       UA.DESCRICAO_DEFICIENCIA AS descricaoDeficiencia, 
                       UA.NECESSITA_ADAPTACAO AS necessitaAdaptacao, 
                       UA.DESCRICAO_ADAPTACAO AS descricaoAdaptacao, 
                       I.SITUACAO AS situacaoInscricao, 
                       TO_CHAR(I.CRIADO_EM, 'YYYY-MM-DD') AS dataInscricao,
                       TO_CHAR(I.CRIADO_EM, 'HH24:MI:SS') AS horaInscricao,
                       NULL AS situacaoConclusaoCursista, 
                       U.EMAIL_EDUCACIONAL AS email,
                       U.EMAIL AS emailNaoEducacional,
                       U.TELEFONE AS telefone,
                       ROW_NUMBER() OVER (PARTITION BY P.ID, PT.ID, U.ID ORDER BY 
                           CASE
                               WHEN CURRENT_DATE BETWEEN P.DATA_REALIZACAO_INICIO AND P.DATA_REALIZACAO_FIM THEN 1
                               WHEN P.DATA_REALIZACAO_INICIO > CURRENT_DATE THEN 2
                               WHEN P.DATA_REALIZACAO_FIM < CURRENT_DATE THEN 3
                               ELSE 4
                           END,
                           CASE WHEN P.DATA_REALIZACAO_INICIO > CURRENT_DATE THEN P.DATA_REALIZACAO_INICIO END ASC,
                           CASE WHEN P.DATA_REALIZACAO_FIM < CURRENT_DATE THEN P.DATA_REALIZACAO_FIM END DESC
                       ) AS rn
                FROM   PUBLIC.PROPOSTA P 
                       INNER JOIN PUBLIC.AREA_PROMOTORA AP ON AP.ID = P.AREA_PROMOTORA_ID 
                       INNER JOIN PUBLIC.PROPOSTA_TURMA PT ON PT.PROPOSTA_ID = P.ID 
                       INNER JOIN PUBLIC.INSCRICAO I ON I.PROPOSTA_TURMA_ID = PT.ID 
                       INNER JOIN PUBLIC.USUARIO U ON U.ID = I.USUARIO_ID 
                       LEFT JOIN PUBLIC.USUARIO_ACESSIBILIDADE UA ON UA.USUARIO_ID = U.ID AND NOT UA.EXCLUIDO 
                -- Cargo da inscrição do usuário
                       LEFT JOIN PUBLIC.CARGO_FUNCAO CF_CARGO ON CF_CARGO.ID = I.CARGO_ID 
                -- Modalidade / Etapa
                       LEFT JOIN PUBLIC.PROPOSTA_MODALIDADE PM ON PM.PROPOSTA_ID = P.ID AND NOT PM.EXCLUIDO 
                -- Função específica da proposta
                       LEFT JOIN PUBLIC.PROPOSTA_FUNCAO_ESPECIFICA PFE ON PFE.PROPOSTA_ID = P.ID AND NOT PFE.EXCLUIDO 
                       LEFT JOIN PUBLIC.CARGO_FUNCAO CF_FUNC_PROP ON CF_FUNC_PROP.ID = PFE.CARGO_FUNCAO_ID 
                -- Componente curricular
                       LEFT JOIN PUBLIC.PROPOSTA_COMPONENTE_CURRICULAR PCC ON PCC.PROPOSTA_ID = P.ID AND NOT PCC.EXCLUIDO 
                       LEFT JOIN PUBLIC.COMPONENTE_CURRICULAR CC ON CC.ID = PCC.COMPONENTE_CURRICULAR_ID 
                -- Ano / Etapa
                       LEFT JOIN PUBLIC.PROPOSTA_ANO_TURMA PAT ON PAT.PROPOSTA_ID = P.ID AND NOT PAT.EXCLUIDO 
                       LEFT JOIN PUBLIC.ANO_TURMA AT ON AT.ID = PAT.ANO_TURMA_ID 
                -- Unidade educacional do cursista
                       LEFT JOIN PUBLIC.UE ON UE.CODIGO_UE = I.CARGO_UE_CODIGO 
                       LEFT JOIN PUBLIC.DRE D ON D.ID = UE.DRE_ID 
                WHERE  NOT P.EXCLUIDO
                  AND  NOT PT.EXCLUIDO
            )
            SELECT codigoFormacao,
                   codigoHomologacao,
                   nomeFormacao,
                   areaPromotora,
                   dre,
                   ue,
                   dataRealizacaoInicio,
                   dataRealizacaoFim,
                   situacaoFormacao,
                   modalidadeFormativa,
                   publicoAlvo,
                   funcaoEspecifica,
                   etapaModalidade,
                   anoEtapa,
                   componenteCurricular,
                   turma,
                   rfCpf,
                   nomeCursista,
                   telefone,
                   pcd,
                   descricaoDeficiencia,
                   necessitaAdaptacao,
                   descricaoAdaptacao,
                   situacaoInscricao,
                   dataInscricao,
                   horaInscricao,
                   situacaoConclusaoCursista,
                   email,
                   emailNaoEducacional
            FROM   inscritos_rankeados
            """;

        public const string QueryOrderbyInscritosPorFormacao =
            """
            ORDER BY
                -- 1. CRIAÇÃO DAS PRIORIDADES DE ORDENAÇÃO
                CASE
                    -- Peso 1. Em andamento (Hoje está entre a data de início e fim da formação)
                    WHEN CURRENT_DATE BETWEEN dataRealizacaoInicio AND dataRealizacaoFim THEN 1

                    -- Peso 2. Futuras (Ainda vai começar)
                    WHEN dataRealizacaoInicio > CURRENT_DATE THEN 2

                    -- Peso 3. Passadas (Já terminaram)
                    WHEN dataRealizacaoFim < CURRENT_DATE THEN 3

                    -- Peso 4. Caso haja algum registro com data de realização nula, considerar por último
                    ELSE 4
                END,

                -- 2. DESEMPATE INTELIGENTE DENTRO DE CADA PRIORIDADE

                -- Se for Futuro (Peso 2), queremos os MAIS PRÓXIMOS primeiro
                CASE WHEN dataRealizacaoInicio > CURRENT_DATE THEN dataRealizacaoInicio END ASC,

                -- Se for Passado (Peso 3), queremos os MAIS RECENTES primeiro
                CASE WHEN dataRealizacaoFim < CURRENT_DATE THEN dataRealizacaoFim END DESC,

                -- 3. ORDENAÇÃO PADRÃO
                dataRealizacaoInicio ASC,
                nomeFormacao ASC,
                codigoHomologacao ASC,
                nomeCursista ASC,
                rfCpf ASC
            """;
    }
}
