namespace SME.ConectaFormacao.Infra.Servicos.Eol
{
    public class UnidadeEol
    {
        public string Codigo { get; set; }
        public string Sigla { get; set; }
        public string NomeUnidade { get; set; }
        public UnidadeEolTipo Tipo { get; set; }
        public string CodigoReferencia { get; set; }
    }

    public enum UnidadeEolTipo
    {
        Escola = 1,
        UnidadeExterna = 2,
        Administrativo = 3,
        Instituicao = 4
    }
}