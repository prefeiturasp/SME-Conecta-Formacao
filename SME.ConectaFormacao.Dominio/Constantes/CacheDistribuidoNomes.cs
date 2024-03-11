namespace SME.ConectaFormacao.Dominio.Constantes
{
    public class CacheDistribuidoNomes
    {
        /// <summary>
        /// 0 - login usuário
        /// </summary>
        public const string UsuarioLogado = "usuario-logado:{0}";
        /// <summary>
        /// 0 - Tipo
        /// 1 - Outros
        /// </summary>
        public const string CargoFuncao = "cargo-funcao:{0}:outros-{1}";
        public const string CargoFuncaoOutros = "cargo-funcao:outros";

        public const string ParametroSistemaTipo = "parametro-sistema-tipo:{0}";
        public const string AreaPromotora = "area-promotora";
        public const string PalavraChave = "palavra-chave";
        /// <summary>
        /// 0 - Código da proposta
        /// </summary>
        public const string FormacaoResumida = "formacao-resumida:{0}";
        /// <summary>
        /// 0 - Código da proposta
        /// </summary>
        public const string FormacaoDetalhada = "formacao-detalhada:{0}";
        /// <summary>
        /// 0 - Registro funcional
        /// </summary>
        public const string CargosFuncoesDresEolFuncionario = "cargos-funcao-dre-eol-funcionario:{0}";
        /// <summary>
        /// 0 - Código da proposta
        /// </summary>
        public const string Proposta = "proposta:{0}";
        /// <summary>
        /// 0 - Código da turma da proposta
        /// </summary>
        public const string PropostaTurma = "proposta-turma:{0}";
        /// <summary>
        /// 0 - Código da turma da proposta
        /// </summary>
        public const string PropostaTurmaDre = "proposta-turma:{0}:dre";
        /// <summary>
        /// 0 - Código da proposta
        /// </summary>
        public const string PropostaPublicoAlvo = "proposta:{0}:publico-alvo";
        /// <summary>
        /// 0 - Código da proposta
        /// </summary>
        public const string PropostaFuncaoEspecifica = "proposta:{0}:funcao-especifica";

        /// <summary>
        /// 0 - Login (rf)
        /// </summary>
        public const string Usuario = "usuario:{0}";
        public const string DreTodos = "dre:todos";
        public const string UnidadeEol = "unidade-eol:{0}";
        public const string DashboardProposta = "dashboard:{0}";
        public const string DashboardTotalPorTipo = "dashboard-total-por-tipo";
    }
}
