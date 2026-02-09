namespace SME.ConectaFormacao.Infra.Servicos.Relatorio.Constantes
{
    public static class EndpointRelatoriosConstants
    {
        public const string RELATORIO_LAUDA_PUBLICACAO = "v1/conecta/prosposta/{0}/lauda-publicacao";
        public const string RELATORIO_LAUDA_COMPLETA = "v1/conecta/prosposta/{0}/lauda-completa";
        public const string RELATORIO_CERTIFICADO_CODAF = "v1/conecta/gerar-certificado-codaf";
        public const string RELATORIO_CODAF = "v1/conecta/codaf/{0}/gerar-planilha";
    }
}
