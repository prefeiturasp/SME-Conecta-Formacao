using FluentAssertions;
using SME.ConectaFormacao.Infra.Dados.Queries;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Queries
{
    public class RelatoriosInscritosQueriesTeste
    {      
        private static readonly string[] LeftJoinSplit = ["LEFT JOIN"];
        private static readonly string[] InnerJoinSplit = ["INNER JOIN"];
        private static readonly string[] SelectFinalSplit = ["SELECT codigoFormacao,"];

        #region ObterInscritosPorFormacao - Testes de Estrutura SQL

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterQueryCompleta()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .NotBeNullOrEmpty()
                .And.Contain("WITH inscritos_rankeados AS")
                .And.Contain("SELECT")
                .And.Contain("FROM");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterCTEComRankeamento()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("ROW_NUMBER()")
                .And.Contain("PARTITION BY P.ID, PT.ID, U.ID")
                .And.Contain("ORDER BY");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterAllJoinsNecessarios()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("INNER JOIN PUBLIC.AREA_PROMOTORA")
                .And.Contain("INNER JOIN PUBLIC.PROPOSTA_TURMA")
                .And.Contain("INNER JOIN PUBLIC.INSCRICAO")
                .And.Contain("INNER JOIN PUBLIC.USUARIO");

            query.Should()
                .Contain("LEFT JOIN PUBLIC.USUARIO_ACESSIBILIDADE")
                .And.Contain("LEFT JOIN PUBLIC.CARGO_FUNCAO CF_CARGO")
                .And.Contain("LEFT JOIN PUBLIC.PROPOSTA_MODALIDADE")
                .And.Contain("LEFT JOIN PUBLIC.PROPOSTA_FUNCAO_ESPECIFICA")
                .And.Contain("LEFT JOIN PUBLIC.PROPOSTA_COMPONENTE_CURRICULAR")
                .And.Contain("LEFT JOIN PUBLIC.COMPONENTE_CURRICULAR")
                .And.Contain("LEFT JOIN PUBLIC.PROPOSTA_ANO_TURMA")
                .And.Contain("LEFT JOIN PUBLIC.ANO_TURMA")
                .And.Contain("LEFT JOIN PUBLIC.UE")
                .And.Contain("LEFT JOIN PUBLIC.DRE");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterFiltrosWhereObrigatorios()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("WHERE  NOT P.EXCLUIDO")
                .And.Contain("AND  NOT PT.EXCLUIDO");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterAllCamposSelect()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("P.ID AS codigoFormacao")
                .And.Contain("P.NUMERO_HOMOLOGACAO AS codigoHomologacao")
                .And.Contain("P.NOME_FORMACAO AS nomeFormacao")
                .And.Contain("AP.NOME AS areaPromotora")
                .And.Contain("D.NOME AS dre")
                .And.Contain("UE.NOME_ESCOLA AS ue")
                .And.Contain("P.DATA_REALIZACAO_INICIO as dataRealizacaoInicio")
                .And.Contain("P.DATA_REALIZACAO_FIM as dataRealizacaoFim")
                .And.Contain("P.SITUACAO AS situacaoFormacao")
                .And.Contain("P.FORMATO AS modalidadeFormativa")
                .And.Contain("TO_CHAR(I.CRIADO_EM, 'YYYY-MM-DD') AS dataInscricao")
                .And.Contain("TO_CHAR(I.CRIADO_EM, 'HH24:MI') AS horaInscricao");

            query.Should()
                .Contain("CASE")
                .And.Contain("WHEN U.TIPO = 2 THEN 'Estudante de Estágio'")
                .And.Contain("WHEN U.TIPO = 3 THEN 'Funcionário de Unidades Parceiras'")
                .And.Contain("ELSE CF_CARGO.NOME")
                .And.Contain("END AS publicoAlvo");

            query.Should()
                .Contain("U.LOGIN AS rfCpf")
                .And.Contain("U.NOME AS nomeCursista")
                .And.Contain("U.EMAIL_EDUCACIONAL AS email");

            query.Should()
                .Contain("UA.POSSUI_DEFICIENCIA AS pcd")
                .And.Contain("UA.DESCRICAO_DEFICIENCIA AS descricaoDeficiencia")
                .And.Contain("UA.NECESSITA_ADAPTACAO AS necessitaAdaptacao")
                .And.Contain("UA.DESCRICAO_ADAPTACAO AS descricaoAdaptacao");

            query.Should()
                .Contain("I.SITUACAO AS situacaoInscricao")
                .And.Contain("NULL AS situacaoConclusaoCursista");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterCaseParaTipoCursista()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            var caseStart = query.IndexOf("CASE", StringComparison.Ordinal);
            var caseEnd = query.IndexOf("END AS publicoAlvo", caseStart, StringComparison.Ordinal);
            var caseExpression = query[caseStart..caseEnd];

            caseExpression.Should()
                .Contain("U.TIPO = 2");

            caseExpression.Should()
                .Contain("U.TIPO = 3");
        }

        [Fact]
        public void ObterInscritosPorFormacao_CasePuablicoAlvoDeveConterTodosCaminhos()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            var caseInicio = query.IndexOf("CASE", StringComparison.Ordinal);
            var caseFim = query.IndexOf("END AS publicoAlvo", caseInicio, StringComparison.Ordinal) > caseInicio;
            caseFim.Should().BeTrue();

            var casePart = query[caseInicio..(query.IndexOf("END AS publicoAlvo", caseInicio, StringComparison.Ordinal) + 20)];

            casePart.Should()
                .Contain("WHEN U.TIPO = 2")
                .And.Contain("WHEN U.TIPO = 3")
                .And.Contain("ELSE CF_CARGO.NOME");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterFuncaoFENoSelect()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("CF_FUNC_PROP.NOME AS funcaoEspecifica")
                .And.Contain("PM.MODALIDADE AS etapaModalidade")
                .And.Contain("AT.DESCRICAO AS anoEtapa")
                .And.Contain("CC.NOME AS componenteCurricular")
                .And.Contain("PT.NOME AS turma");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterCondicoesFiltroAcessibilidade()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("LEFT JOIN PUBLIC.USUARIO_ACESSIBILIDADE UA ON UA.USUARIO_ID = U.ID AND NOT UA.EXCLUIDO");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterJoinUEEDRE()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("LEFT JOIN PUBLIC.UE ON UE.CODIGO_UE = I.CARGO_UE_CODIGO")
                .And.Contain("LEFT JOIN PUBLIC.DRE D ON D.ID = UE.DRE_ID");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterRankeamentoPorPriorizacao()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("WHEN CURRENT_DATE BETWEEN P.DATA_REALIZACAO_INICIO AND P.DATA_REALIZACAO_FIM THEN 1");

            query.Should()
                .Contain("WHEN P.DATA_REALIZACAO_INICIO > CURRENT_DATE THEN 2");

            query.Should()
                .Contain("WHEN P.DATA_REALIZACAO_FIM < CURRENT_DATE THEN 3");

            query.Should()
                .Contain("ELSE 4");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterDesempateOrdenacaoParaFuturo()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("CASE WHEN P.DATA_REALIZACAO_INICIO > CURRENT_DATE THEN P.DATA_REALIZACAO_INICIO END ASC");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterDesempateOrdenacaoParaPassado()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("CASE WHEN P.DATA_REALIZACAO_FIM < CURRENT_DATE THEN P.DATA_REALIZACAO_FIM END DESC");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterSelectFinalComAllCampos()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            var indexCTE = query.IndexOf("WITH inscritos_rankeados");
            var indexSelectFinal = query.IndexOf("SELECT codigoFormacao,");

            indexSelectFinal.Should()
                .BeGreaterThan(indexCTE)
                .And.BeGreaterThanOrEqualTo(0);

            query.Substring(indexSelectFinal).Should()
                .Contain("FROM   inscritos_rankeados");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterAllColunasNoSelectFinal()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            var selectFinalSection = query.Substring(query.IndexOf(SelectFinalSplit[0]));

            selectFinalSection.Should()
                .Contain("codigoFormacao")
                .And.Contain("codigoHomologacao")
                .And.Contain("nomeFormacao")
                .And.Contain("areaPromotora")
                .And.Contain("dre")
                .And.Contain("ue")
                .And.Contain("dataRealizacaoInicio")
                .And.Contain("dataRealizacaoFim")
                .And.Contain("situacaoFormacao")
                .And.Contain("modalidadeFormativa")
                .And.Contain("publicoAlvo")
                .And.Contain("funcaoEspecifica")
                .And.Contain("etapaModalidade")
                .And.Contain("anoEtapa")
                .And.Contain("componenteCurricular")
                .And.Contain("turma")
                .And.Contain("rfCpf")
                .And.Contain("nomeCursista")
                .And.Contain("pcd")
                .And.Contain("descricaoDeficiencia")
                .And.Contain("necessitaAdaptacao")
                .And.Contain("descricaoAdaptacao")
                .And.Contain("situacaoInscricao")
                .And.Contain("dataInscricao")
                .And.Contain("horaInscricao")
                .And.Contain("situacaoConclusaoCursista")
                .And.Contain("email");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterSelectFinalDoCTE()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            var indexSelectDoCTE = query.LastIndexOf("SELECT");
            var indexFromCTE = query.LastIndexOf("FROM   inscritos_rankeados");

            indexFromCTE.Should()
                .BeGreaterThan(indexSelectDoCTE);

            query.Substring(indexSelectDoCTE).Should()
                .Contain("FROM   inscritos_rankeados");
        }

        #endregion

        #region QueryOrderbyInscritosPorFormacao - Testes de Ordenação

        [Fact]
        public void QueryOrderbyInscritosPorFormacao_DeveConterClausulaOrder()
        {
            var query = RelatoriosInscritosQueries.QueryOrderbyInscritosPorFormacao;

            query.Should()
                .NotBeNullOrEmpty()
                .And.StartWith("ORDER BY");
        }

        [Fact]
        public void QueryOrderbyInscritosPorFormacao_DeveConterCasePriorizacao()
        {
            var query = RelatoriosInscritosQueries.QueryOrderbyInscritosPorFormacao;

            query.Should()
                .Contain("CASE")
                .And.Contain("WHEN CURRENT_DATE BETWEEN dataRealizacaoInicio AND dataRealizacaoFim THEN 1");
        }

        [Fact]
        public void QueryOrderbyInscritosPorFormacao_DeveConterPesoFormacaoEmAndamento()
        {
            var query = RelatoriosInscritosQueries.QueryOrderbyInscritosPorFormacao;

            query.Should()
                .Contain("WHEN CURRENT_DATE BETWEEN dataRealizacaoInicio AND dataRealizacaoFim THEN 1");
        }

        [Fact]
        public void QueryOrderbyInscritosPorFormacao_DeveConterPesoFormacaoFutura()
        {
            var query = RelatoriosInscritosQueries.QueryOrderbyInscritosPorFormacao;

            query.Should()
                .Contain("WHEN dataRealizacaoInicio > CURRENT_DATE THEN 2");
        }

        [Fact]
        public void QueryOrderbyInscritosPorFormacao_DeveConterPesoFormacaoPassada()
        {
            var query = RelatoriosInscritosQueries.QueryOrderbyInscritosPorFormacao;

            query.Should()
                .Contain("WHEN dataRealizacaoFim < CURRENT_DATE THEN 3");
        }

        [Fact]
        public void QueryOrderbyInscritosPorFormacao_DeveConterPesoPadrao()
        {
            var query = RelatoriosInscritosQueries.QueryOrderbyInscritosPorFormacao;

            query.Should()
                .Contain("ELSE 4");
        }

        [Fact]
        public void QueryOrderbyInscritosPorFormacao_DeveConterDesempateFuturo()
        {
            var query = RelatoriosInscritosQueries.QueryOrderbyInscritosPorFormacao;

            query.Should()
                .Contain("CASE WHEN dataRealizacaoInicio > CURRENT_DATE THEN dataRealizacaoInicio END ASC");
        }

        [Fact]
        public void QueryOrderbyInscritosPorFormacao_DeveConterDesemplatePassado()
        {
            var query = RelatoriosInscritosQueries.QueryOrderbyInscritosPorFormacao;

            query.Should()
                .Contain("CASE WHEN dataRealizacaoFim < CURRENT_DATE THEN dataRealizacaoFim END DESC");
        }

        [Fact]
        public void QueryOrderbyInscritosPorFormacao_DeveConterOrdenacaoPadrao()
        {
            var query = RelatoriosInscritosQueries.QueryOrderbyInscritosPorFormacao;

            query.Should()
                .Contain("dataRealizacaoInicio ASC")
                .And.Contain("nomeFormacao ASC")
                .And.Contain("codigoHomologacao ASC")
                .And.Contain("nomeCursista ASC")
                .And.Contain("rfCpf ASC");
        }

        [Fact]
        public void QueryOrderbyInscritosPorFormacao_DeveConterOrdenacaoCorretaFormacaoPassada()
        {
            var query = RelatoriosInscritosQueries.QueryOrderbyInscritosPorFormacao;

           var indexPassado = query.IndexOf("WHEN dataRealizacaoFim < CURRENT_DATE");
            var indexCasePassado = query.Substring(indexPassado).IndexOf("THEN dataRealizacaoFim END DESC");

            indexCasePassado.Should()
                .BeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public void QueryOrderbyInscritosPorFormacao_DeveConterOrdenacaoCorretaFormacaoFutura()
        {
            var query = RelatoriosInscritosQueries.QueryOrderbyInscritosPorFormacao;

            var indexFuturo = query.IndexOf("WHEN dataRealizacaoInicio > CURRENT_DATE");
            var indexCaseFuturo = query.Substring(indexFuturo).IndexOf("THEN dataRealizacaoInicio END ASC");

            indexCaseFuturo.Should()
                .BeGreaterThanOrEqualTo(0);
        }

        #endregion

        #region Testes de Integração e Completude

        [Fact]
        public void RelatoriosInscritosQueries_DeveConterDuasConsultasDefinidas()
        {
            var tipo = typeof(RelatoriosInscritosQueries);

            var campos = tipo.GetFields(
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.Static | 
                System.Reflection.BindingFlags.IgnoreCase);

            campos.Length.Should()
                .BeGreaterThanOrEqualTo(2);

            var consultasDefinidas = campos
                .Where(f => f.FieldType == typeof(string))
                .Select(f => f.Name)
                .ToList();

            consultasDefinidas.Should()
                .Contain("ObterInscritosPorFormacao")
                .And.Contain("QueryOrderbyInscritosPorFormacao");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterAllTableAliases()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain(" P.")       // PROPOSTA
                .And.Contain(" AP.")  // AREA_PROMOTORA
                .And.Contain(" PT.")  // PROPOSTA_TURMA
                .And.Contain(" I.")   // INSCRICAO
                .And.Contain(" U.")   // USUARIO
                .And.Contain(" UA.")  // USUARIO_ACESSIBILIDADE
                .And.Contain(" D.")   // DRE
                .And.Contain(" UE."); // UE
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterComentariosExplicativos()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("-- Cargo da inscrição do usuário")
                .And.Contain("-- Modalidade / Etapa")
                .And.Contain("-- Função específica da proposta")
                .And.Contain("-- Componente curricular")
                .And.Contain("-- Ano / Etapa")
                .And.Contain("-- Unidade educacional do cursista");
        }

        [Fact]
        public void QueryOrderbyInscritosPorFormacao_DeveConterComentariosExplicativos()
        {
            var query = RelatoriosInscritosQueries.QueryOrderbyInscritosPorFormacao;

            query.Should()
                .Contain("-- 1. CRIAÇÃO DAS PRIORIDADES DE ORDENAÇÃO")
                .And.Contain("-- 2. DESEMPATE INTELIGENTE DENTRO DE CADA PRIORIDADE")
                .And.Contain("-- 3. ORDENAÇÃO PADRÃO");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterNullParaSituacaoConclusao()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("NULL AS situacaoConclusaoCursista");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveExcluirRegistrosDeletados()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("WHERE  NOT P.EXCLUIDO")
                .And.Contain("AND  NOT PT.EXCLUIDO");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveExcluirAcessibilidadeDeletada()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("LEFT JOIN PUBLIC.USUARIO_ACESSIBILIDADE UA ON UA.USUARIO_ID = U.ID AND NOT UA.EXCLUIDO");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveExcluirModalidadeDeletada()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("LEFT JOIN PUBLIC.PROPOSTA_MODALIDADE PM ON PM.PROPOSTA_ID = P.ID AND NOT PM.EXCLUIDO");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveExcluirFuncaoEspecificaDeletada()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("LEFT JOIN PUBLIC.PROPOSTA_FUNCAO_ESPECIFICA PFE ON PFE.PROPOSTA_ID = P.ID AND NOT PFE.EXCLUIDO");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveExcluirComponenteCurricularDeletado()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("LEFT JOIN PUBLIC.PROPOSTA_COMPONENTE_CURRICULAR PCC ON PCC.PROPOSTA_ID = P.ID AND NOT PCC.EXCLUIDO");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveExcluirAnoTurmaDeletado()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("LEFT JOIN PUBLIC.PROPOSTA_ANO_TURMA PAT ON PAT.PROPOSTA_ID = P.ID AND NOT PAT.EXCLUIDO");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterAliasParaCargosDistintos()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("CF_CARGO")     
                .And.Contain("CF_FUNC_PROP"); 
        }

        [Fact]
        public void ObterInscritosPorFormacao_RankeamentoDeveParticionarPorFormacaoTurmaUsuario()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("PARTITION BY P.ID, PT.ID, U.ID");
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveRetornarRnNaSelecao()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("AS rn");
        }

        #endregion

        #region Testes de Sintaxe e Validação

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterSintaxeSQLValida()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            var temWith = query.Contains("WITH");
            var temSelectInicial = query.Contains("SELECT P.ID");
            var temSelectFinal = query.Contains("SELECT codigoFormacao");
            var temFrom = query.Contains("FROM");

            temWith.Should().BeTrue();
            temSelectInicial.Should().BeTrue();
            temSelectFinal.Should().BeTrue();
            temFrom.Should().BeTrue();
        }

        [Fact]
        public void QueryOrderbyInscritosPorFormacao_DeveConterSintaxeSQLValida()
        {
            var query = RelatoriosInscritosQueries.QueryOrderbyInscritosPorFormacao;

            query.Should()
                .StartWith("ORDER BY")
                .And.Contain("CASE")
                .And.Contain("WHEN")
                .And.Contain("THEN")
                .And.Contain("ELSE")
                .And.Contain("END");
        }

        #endregion

        #region Testes de Cobertura Total

        [Fact]
        public void RelatoriosInscritosQueries_DeveConterAllCamposAcessivelidade()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("UA.POSSUI_DEFICIENCIA")
                .And.Contain("UA.DESCRICAO_DEFICIENCIA")
                .And.Contain("UA.NECESSITA_ADAPTACAO")
                .And.Contain("UA.DESCRICAO_ADAPTACAO");
        }

        [Fact]
        public void RelatoriosInscritosQueries_DeveConterAllCamposFormacao()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("P.ID AS codigoFormacao")
                .And.Contain("P.NUMERO_HOMOLOGACAO AS codigoHomologacao")
                .And.Contain("P.NOME_FORMACAO AS nomeFormacao")
                .And.Contain("P.DATA_REALIZACAO_INICIO")
                .And.Contain("P.DATA_REALIZACAO_FIM")
                .And.Contain("P.SITUACAO AS situacaoFormacao")
                .And.Contain("P.FORMATO AS modalidadeFormativa")
                .And.Contain("P.AREA_PROMOTORA_ID");
        }

        [Fact]
        public void RelatoriosInscritosQueries_DeveConterAllCamposUsuario()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            query.Should()
                .Contain("U.LOGIN AS rfCpf")
                .And.Contain("U.NOME AS nomeCursista")
                .And.Contain("U.TIPO")
                .And.Contain("U.EMAIL_EDUCACIONAL AS email");
        }

        [Fact]
        public void RelatoriosInscritosQueries_DeveConterTodosOsLeftJoins()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            var leftJoinCount = query.Split(LeftJoinSplit, System.StringSplitOptions.None).Length - 1;

            leftJoinCount.Should()
                .BeGreaterThanOrEqualTo(10);
        }

        [Fact]
        public void ObterInscritosPorFormacao_DeveConterAllInnerJoins()
        {
            var query = RelatoriosInscritosQueries.ObterInscritosPorFormacao;

            var innerJoinCount = query.Split(InnerJoinSplit, System.StringSplitOptions.None).Length - 1;

            innerJoinCount.Should()
                .BeGreaterThanOrEqualTo(4);
        }

        #endregion
    }
}
