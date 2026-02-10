namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafLogRemessaConclusao
    {
        public long Id { get; set; }
        public long CodafListaPresencaId { get; set; }
        public DateTime DataGeracao { get; set; }
        public string NomeArquivoGerado { get; set; } = null!;
        public string HashArquivo { get; set; } = null!;
        public int QuantidadeRegistros { get; set; }
        public string CriadoLogin { get; set; } = null!;
    }
}
