using SME.ConectaFormacao.Infra.Dados.Queries;
using System.Text.RegularExpressions;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Queries
{
    public class CodafDeclaracaoQueriesTestes
    {
        [Theory]
        [MemberData(nameof(ObterQueries))]
        public void Queries_DevemEstarPreenchidas(string query)
        {
            // Assert
            Assert.False(string.IsNullOrWhiteSpace(query));
        }

        [Fact]
        public void ObterDadosParaEmissao_DeveConterEstruturaEsperada()
        {
            // Arrange
            var query = CodafDeclaracaoQueries.ObterDadosParaEmissao;

            // Assert
            DeveConter(
                query,
                "SELECT",
                "PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO",
                "PUBLIC.PROPOSTA_TURMA",
                "PUBLIC.PROPOSTA",
                "PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO",
                "PUBLIC.INSCRICAO",
                "PUBLIC.USUARIO",
                "PUBLIC.CODAF_DECLARACOES",
                "PUBLIC.DRE",
                "PUBLIC.COORDENADORIA",
                "@codafNaoHomologadoId",
                "P.CURSO_COM_CERTIFICADO = false",
                "UNION ALL");
        }

        [Fact]
        public void ObterDadosParaEmissao_DeveBuscarCursistasParticipantes()
        {
            // Arrange
            var query = CodafDeclaracaoQueries.ObterDadosParaEmissao;

            // Assert
            DeveConter(
                query,
                "1 AS tipoParticipacao",
                "CILP.PARTICIPOU",
                "NOT CILP.EXCLUIDO",
                "NOT CLP.EXCLUIDO",
                "CILP.INSCRICAO_ID AS inscricaoId",
                "U.NOME AS nomeCompleto",
                "U.EMAIL AS emailUsuario");
        }

        [Fact]
        public void ObterDadosParaEmissao_DeveBuscarRegentes()
        {
            // Arrange
            var query = CodafDeclaracaoQueries.ObterDadosParaEmissao;

            // Assert
            DeveConter(
                query,
                "2 AS tipoParticipacao",
                "PUBLIC.PROPOSTA_REGENTE_TURMA",
                "PUBLIC.PROPOSTA_REGENTE",
                "PR.NOME_REGENTE AS nomeCompleto",
                "coalesce(PR.REGISTRO_FUNCIONAL, PR.CPF) AS documento",
                "PR.REGISTRO_FUNCIONAL IS NOT NULL AS temRf",
                "NOT PRT.EXCLUIDO",
                "NOT PR.EXCLUIDO");
        }

        [Fact]
        public void ObterDadosParaEmissao_DeveRetornarDadosDoEmissor()
        {
            // Arrange
            var query = CodafDeclaracaoQueries.ObterDadosParaEmissao;

            // Assert
            DeveConter(
                query,
                "P.TIPO_EMISSOR AS tipoEmissor",
                "CASE WHEN P.TIPO_EMISSOR = 2 THEN C_EMISSOR.NOME ELSE D_EMISSOR.NOME END AS emissor",
                "CASE WHEN P.TIPO_EMISSOR = 2 THEN C_EMISSOR.SIGLA ELSE NULL END AS emissorSigla",
                "D_EMISSOR.ID = P.ID_EMISSOR",
                "C_EMISSOR.ID = P.ID_EMISSOR");
        }

        [Fact]
        public void AtualizarStatusProcessamento_DeveAtualizarTodosOsCamposEsperados()
        {
            // Arrange
            var query = CodafDeclaracaoQueries.AtualizarStatusProcessamento;

            // Assert
            DeveConter(
                query,
                "UPDATE PUBLIC.CODAF_DECLARACOES",
                "STATUS_PROCESSAMENTO = @statusProcessamento",
                "CHAVE_OBJETO_ARMAZENAMENTO = @chaveObjetoArmazenamento",
                "ERRO_PROCESSAMENTO = @erroProcessamento",
                "ALTERADO_EM = NOW()",
                "ALTERADO_POR = 'WORKER'",
                "WHERE ID = @id");
        }

        [Fact]
        public void ObterParaProcessamento_DeveSelecionarLotePendente()
        {
            // Arrange
            var query = CodafDeclaracaoQueries.ObterParaProcessamento;

            // Assert
            DeveConter(
                query,
                "WITH batch_para_processar AS",
                "PUBLIC.CODAF_DECLARACOES",
                "CC.STATUS_PROCESSAMENTO = @statusPendente",
                "ORDER BY id ASC",
                "LIMIT @tamanhoLote",
                "FOR UPDATE SKIP LOCKED");
        }

        [Fact]
        public void ObterParaProcessamento_DeveMarcarRegistrosComoProcessando()
        {
            // Arrange
            var query = CodafDeclaracaoQueries.ObterParaProcessamento;

            // Assert
            DeveConter(
                query,
                "declaracoes_atualizadas AS",
                "UPDATE PUBLIC.CODAF_DECLARACOES C",
                "STATUS_PROCESSAMENTO = @statusProcessando",
                "ALTERADO_EM = NOW()",
                "ALTERADO_POR = 'WORKER'",
                "FROM batch_para_processar B",
                "WHERE C.id = B.id",
                "RETURNING C.ID",
                "C.CODIGO_DECLARACAO",
                "C.HTML_CONTENT_SNAPSHOT");
        }

        [Fact]
        public void ObterParaProcessamento_DeveUtilizarSkipLocked()
        {
            // Arrange
            var query = CodafDeclaracaoQueries.ObterParaProcessamento;

            // Assert
            DeveConter(
                query,
                "FOR UPDATE SKIP LOCKED");

            Assert.Equal(
                1,
                QuantidadeOcorrencias(
                    query,
                    "FOR UPDATE SKIP LOCKED"));
        }

        [Fact]
        public void ObterParaProcessamento_DeveBuscarCursistas()
        {
            // Arrange
            var query = CodafDeclaracaoQueries.ObterParaProcessamento;

            // Assert
            DeveConter(
                query,
                "CA.CODIGO_DECLARACAO AS codigoDeclaracao",
                "CA.HTML_CONTENT_SNAPSHOT AS htmlContentSnapshot",
                "U.NOME AS nomeCompleto",
                "(U.LOGIN <> U.CPF) AS temRf",
                "1 AS tipoParticipacao",
                "P.NOME_FORMACAO AS nomeFormacao",
                "U.EMAIL AS emailUsuario",
                "CCNHI.PARTICIPOU",
                "NOT CCNHI.EXCLUIDO");
        }

        [Fact]
        public void ObterParaProcessamento_DeveBuscarRegentes()
        {
            // Arrange
            var query = CodafDeclaracaoQueries.ObterParaProcessamento;

            // Assert
            DeveConter(
                query,
                "PR.NOME_REGENTE AS nomeCompleto",
                "TRUE AS temRf",
                "2 AS tipoParticipacao",
                "PUBLIC.PROPOSTA_REGENTE_TURMA",
                "PUBLIC.PROPOSTA_REGENTE",
                "LEFT JOIN PUBLIC.USUARIO",
                "NOT PRT.EXCLUIDO",
                "NOT PR.EXCLUIDO");
        }

        [Fact]
        public void ObterParaProcessamento_DeveUnirCursistasERegentes()
        {
            // Arrange
            var query = CodafDeclaracaoQueries.ObterParaProcessamento;

            // Assert
            DeveConter(
                query,
                "UNION ALL");

            Assert.Equal(
                1,
                QuantidadeOcorrencias(query, "UNION ALL"));
        }

        [Fact]
        public void InserirLoteCopy_DeveConterTabelaEColunasEsperadas()
        {
            // Arrange
            var query = CodafDeclaracaoQueries.InserirLoteCopy;

            // Assert
            DeveConter(
                query,
                "COPY public.codaf_declaracoes",
                "id",
                "codigo_declaracao",
                "codaf_curso_nao_homologado_inscricao_id",
                "codaf_curso_nao_homologado_id",
                "proposta_regente_turma_id",
                "tipo_participacao",
                "data_emissao",
                "html_content_snapshot",
                "metadados_json",
                "status_processamento",
                "tentativas_processamento",
                "criado_em",
                "criado_por",
                "criado_login",
                "excluido",
                "FROM STDIN (FORMAT BINARY)");
        }

        [Fact]
        public void AtualizarCodigoDeclaracaoNoHtml_DeveSubstituirCodigoDeclaracao()
        {
            // Arrange
            var query =
                CodafDeclaracaoQueries.AtualizarCodigoDeclaracaoNoHtml;

            // Assert
            DeveConter(
                query,
                "UPDATE PUBLIC.CODAF_DECLARACOES CC",
                "SET HTML_CONTENT_SNAPSHOT = REPLACE",
                "NUM_CODIGO_DECLARACAO",
                "CAST(CC.CODIGO_DECLARACAO AS TEXT)");
        }

        [Fact]
        public void AtualizarCodigoDeclaracaoNoHtml_DeveSubstituirNumeroHomologacao()
        {
            // Arrange
            var query =
                CodafDeclaracaoQueries.AtualizarCodigoDeclaracaoNoHtml;

            // Assert
            DeveConter(
                query,
                "NUM_HOM_FORMACAO",
                "P.NUMERO_HOMOLOGACAO",
                "PUBLIC.PROPOSTA_TURMA",
                "PUBLIC.PROPOSTA",
                "PT.PROPOSTA_ID = P.ID");
        }

        [Fact]
        public void AtualizarCodigoDeclaracaoNoHtml_DeveAtualizarCursistasERegentesDaTurma()
        {
            // Arrange
            var query =
                CodafDeclaracaoQueries.AtualizarCodigoDeclaracaoNoHtml;

            // Assert
            DeveConter(
                query,
                "CC.CODAF_CURSO_NAO_HOMOLOGADO_ID = @codafNaoHomologadoId",
                "CC.PROPOSTA_REGENTE_TURMA_ID IN",
                "PUBLIC.PROPOSTA_REGENTE_TURMA",
                "PRT.TURMA_ID IN",
                "WHERE ID = @codafNaoHomologadoId",
                "AND NOT CC.EXCLUIDO");
        }

        [Fact]
        public void InativarDeclaracoesAnterioresDeCursistas_DeveRealizarExclusaoLogica()
        {
            // Arrange
            var query =
                CodafDeclaracaoQueries
                    .InativarDeclaracoesAnterioresDeCursistas;

            // Assert
            DeveConter(
                query,
                "UPDATE PUBLIC.CODAF_DECLARACOES",
                "SET EXCLUIDO = TRUE",
                "ALTERADO_EM = NOW()",
                "ALTERADO_POR = @usuarioNome",
                "ALTERADO_LOGIN = @usuarioLogin");
        }

        [Fact]
        public void InativarDeclaracoesAnterioresDeCursistas_DeveFiltrarPelasInscricoes()
        {
            // Arrange
            var query =
                CodafDeclaracaoQueries
                    .InativarDeclaracoesAnterioresDeCursistas;

            // Assert
            DeveConter(
                query,
                "PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO",
                "CC.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO_ID = CSI.ID",
                "NOT CC.EXCLUIDO",
                "CSI.INSCRICAO_ID = ANY(@inscricaoId)");
        }

        public static IEnumerable<object[]> ObterQueries()
        {
            yield return
            [
                CodafDeclaracaoQueries.ObterDadosParaEmissao
            ];

            yield return
            [
                CodafDeclaracaoQueries.AtualizarStatusProcessamento
            ];

            yield return
            [
                CodafDeclaracaoQueries.ObterParaProcessamento
            ];

            yield return
            [
                CodafDeclaracaoQueries.InserirLoteCopy
            ];

            yield return
            [
                CodafDeclaracaoQueries.AtualizarCodigoDeclaracaoNoHtml
            ];

            yield return
            [
                CodafDeclaracaoQueries
                .InativarDeclaracoesAnterioresDeCursistas
            ];
        }

        private static void DeveConter(
            string query,
            params string[] trechosEsperados)
        {
            var queryNormalizada = Normalizar(query);

            foreach (var trecho in trechosEsperados)
            {
                Assert.Contains(
                    Normalizar(trecho),
                    queryNormalizada);
            }
        }

        private static int QuantidadeOcorrencias(
            string query,
            string trecho)
        {
            var queryNormalizada = Normalizar(query);
            var trechoNormalizado = Normalizar(trecho);

            var quantidade = 0;
            var indice = 0;

            while ((indice = queryNormalizada.IndexOf(
                       trechoNormalizado,
                       indice,
                       StringComparison.Ordinal)) >= 0)
            {
                quantidade++;
                indice += trechoNormalizado.Length;
            }

            return quantidade;
        }

        private static string Normalizar(string valor)
        {
            return Regex
                .Replace(valor, @"\s+", " ")
                .Trim()
                .ToUpperInvariant();
        }
    }
}
