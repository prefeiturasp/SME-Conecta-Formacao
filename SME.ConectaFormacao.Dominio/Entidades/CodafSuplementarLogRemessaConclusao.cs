namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafSuplementarLogRemessaConclusao
    {
        public long Id { get; set; }
        public long CodafSuplementarId { get; set; }
        public DateTime DataGeracao { get; set; }
        public string NomeArquivoGerado { get; set; } = null!;
        public string HashArquivo { get; set; } = null!;
        public int QuantidadeRegistros { get; set; }
        public string CriadoLogin { get; set; } = null!;
    }
}
